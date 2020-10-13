using System;
using System.Collections.Generic;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface ITimedInventoryDataManager : IPostConstructable {}
    
    internal class TimedInventoryDataManager : ITimedInventoryDataManager
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;

        private readonly Dictionary<PowerupType, TimedInventorySlot> _inventorySlots = new Dictionary<PowerupType, TimedInventorySlot>();
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameEnd);
            _gameEventManager.Subscribe(GameEvents.Powerup.Pickup, OnPowerupCollected);
        }

        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameEnd);
            _gameEventManager.Unsubscribe(GameEvents.Powerup.Pickup, OnPowerupCollected);
        }
        
        private void OnGameEnd(object[] obj)
        {
            ResetInventory();
        }

        private void OnGameStart(object[] obj)
        {
            ResetInventory();
        }

        private void OnPowerupCollected(object[] obj)
        {
            if(obj?.Length < 1 || !(obj[0] is ITimedPowerupData powerupData)) return;

            if(powerupData.Type == PowerupType.Lives) return;
            
            AddToInventory(powerupData);
        }
        
        private void ResetInventory()
        {
            foreach (var inventorySlot in _inventorySlots)
            {
                inventorySlot.Value.OnEffectBegin -= OnEffectBegin;
                inventorySlot.Value.OnEffectEnd -= OnEffectEnd;
                
                inventorySlot.Value.Reset();
            }
            
            _inventorySlots.Clear();
        }

        private void AddToInventory(ITimedPowerupData powerupData)
        {
            var key = powerupData.Type;

            if (!_inventorySlots.ContainsKey(key))
            {
                _inventorySlots[key] = new TimedInventorySlot(powerupData.Type);
                _inventorySlots[key].OnEffectBegin += OnEffectBegin;
                _inventorySlots[key].OnEffectEnd += OnEffectEnd;
            }
            
            _inventorySlots[key].AddEffect(powerupData.Duration, powerupData.EffectData, powerupData.ResetData);
        }
        
        private void OnEffectBegin(PowerupType powerupType, float data)
        {
            OnPowerupActivated(powerupType, data);
        }

        private void OnEffectEnd(PowerupType powerupType, float data)
        {
            OnPowerupDeactivated(powerupType, data);
        }
        
        private void OnPowerupActivated(PowerupType powerupType, float data)
        {
            Debug.Log($"[{nameof(TimedInventoryDataManager)}] {nameof(OnPowerupActivated)} Type {powerupType} Data {data}");
            
            switch (powerupType)
            {
                case PowerupType.Shrink:
                    new EventCommand(GameEvents.BallScale.Decrement, data).Execute();
                    break;
                case PowerupType.Time:
                    new EventCommand(GameEvents.Time.DecrementScale, data).Execute();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerupType), powerupType, null);
            }
        }
        
        private void OnPowerupDeactivated(PowerupType powerupType, float data)
        {
            Debug.Log($"[{nameof(TimedInventoryDataManager)}] {nameof(OnPowerupDeactivated)} Type {powerupType} Data {data}");
            
            switch (powerupType)
            {
                case PowerupType.Shrink:
                    new EventCommand(GameEvents.BallScale.Increment, data).Execute();
                    break;
                case PowerupType.Time:
                    new EventCommand(GameEvents.Time.IncrementScale, data).Execute();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerupType), powerupType, null);
            }
        }
    }
    
    public static partial class GameConstants
    {
        internal static partial class Powerup
        {
            public const float PowerupDuration = 5f;
        }
    }
}