using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class PowerupDataManager : BaseResourceDataManager<PowerupData>
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        
        protected override string DataPath => GameConstants.DataPaths.Resources.Powerups;
        
        private ShuffleBag _shuffleBag;
        private int _powerupId = -1;

        public override void PostConstruct(params object[] args)
        {
            base.PostConstruct(args);
            
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Subscribe(GameEvents.Gameplay.Pickup, OnPowerupCollected);
        }

        public override void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Pickup, OnPowerupCollected);
            
            base.Dispose();
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
    }
    
    public static partial class GameConstants
    {
        internal static partial class Powerup
        {
            public const float PowerupDuration = 5f;
        }
    }
}