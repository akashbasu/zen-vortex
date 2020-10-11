using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class PowerupDataManager : BaseResourceDataManager<PowerupData>
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        
        protected override string DataPath => GameConstants.DataPaths.Resources.Powerups;
        
        private ShuffleBag _shuffleBag;
        private int _powerupId = -1;

        public override void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            base.Initialize(null, args);
            
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Subscribe(GameEvents.Gameplay.Pickup, OnPowerupCollected);
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);

            onComplete?.Invoke(this);
        }

        private void OnPowerupCollected(object[] obj)
        {
            if(obj?.Length < 1) return;

            var powerup = obj[0] as PowerupData;
            switch (powerup.Type)
            {
                case PowerupType.Lives:
                    new EventCommand(GameEvents.Gameplay.EarnedLife).Execute();
                    break;
                case PowerupType.Shrink:
                    new EventCommand(GameEvents.Gameplay.OverrideSize, powerup.Data, GameConstants.Powerup.PowerupDuration).Execute();
                    break;
                case PowerupType.Time:
                    new EventCommand(GameEvents.Gameplay.OverrideTimeScale, powerup.Data, GameConstants.Powerup.PowerupDuration).Execute();
                    break;
                default:
                    Debug.LogError($"[{nameof(PowerupDataManager)}] {nameof(OnPowerupCollected)} Invalid Powerup type.");
                    break;
            }
        }

        public PowerupData GetNextPowerupData()
        {
            _powerupId = _shuffleBag.Next();
            return _data[_powerupId];
        }
        
        private void OnLevelStart(object[] obj)
        {
            _shuffleBag = new ShuffleBag(_data.Length);
        }
        
        private void OnLevelStop(object[] obj)
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Pickup, OnPowerupCollected);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
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