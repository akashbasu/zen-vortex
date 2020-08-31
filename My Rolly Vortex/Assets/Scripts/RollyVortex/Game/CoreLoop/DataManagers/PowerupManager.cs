using System;
using UnityEngine;

namespace RollyVortex
{
    internal class PowerupManager : IInitializable
    {
        private static ShuffleBag _shuffleBag;
        private static int _powerupId = -1;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Subscribe(GameEvents.Gameplay.Pickup, OnPowerupCollected);
            GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);

            onComplete?.Invoke(this);
        }

        private void OnPowerupCollected(object[] obj)
        {
            if(obj?.Length < 1) return;

            var powerup = obj[0] as PowerupData;
            switch (powerup.Type)
            {
                case PowerupType.Gems:
                    new Command(GameEvents.Gameplay.EarnedLife).Execute();
                    break;
                case PowerupType.Shrink:
                    new Command(GameEvents.Gameplay.OverrideSize, powerup.Data, GameConstants.Powerup.PowerupDuration).Execute();
                    break;
                case PowerupType.Time:
                    new Command(GameEvents.Gameplay.OverrideTimeScale, powerup.Data, GameConstants.Powerup.PowerupDuration).Execute();
                    break;
                case PowerupType.None:
                default:
                    Debug.LogError($"[{nameof(PowerupManager)}] {nameof(OnPowerupCollected)} Invalid Powerup type.");
                    break;
            }
        }

        public static PowerupData GetNextPowerupData()
        {
            _powerupId = _shuffleBag.Next();
            return PowerupDataProvider.PowerupData[_powerupId];
        }
        
        private void OnLevelStart(object[] obj)
        {
            _shuffleBag = new ShuffleBag(PowerupDataProvider.PowerupData.Count);
        }
        
        private void OnLevelStop(object[] obj)
        {
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Unsubscribe(GameEvents.Gameplay.Pickup, OnPowerupCollected);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
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