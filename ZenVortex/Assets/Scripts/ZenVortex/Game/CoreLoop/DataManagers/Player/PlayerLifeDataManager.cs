using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IPlayerLifeDataManager : IPostConstructable
    {
        int LifeCount { get; }
        bool HasExtraLives { get; }
    }
    
    internal class PlayerLifeDataManager : IPlayerLifeDataManager
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;

        public int LifeCount
        {
            get => _livesEarnedInRun;
            private set
            {
                if (_livesEarnedInRun == value) return;
                
                _livesEarnedInRun = value;
                new LivesUpdatedCommand().Execute();
            }
        }

        public bool HasExtraLives => LifeCount > 1;

        private int _livesEarnedInRun;
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Subscribe(GameEvents.Powerup.Pickup, OnPowerupCollected);
            _gameEventManager.Subscribe(GameEvents.Obstacle.Collision, OnCollision);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Unsubscribe(GameEvents.Powerup.Pickup, OnPowerupCollected);
            _gameEventManager.Unsubscribe(GameEvents.Obstacle.Collision, OnCollision);
        }
        
        private void OnPowerupCollected(object[] obj)
        {
            if(obj?.Length < 1 || !(obj[0] is IBasePowerupData powerupData)) return;
            
            if(powerupData.Type != PowerupType.Lives) return;
            
            OnLifeEarned();
        }
        
        private void OnCollision(object[] obj)
        {
            if(obj?.Length < 1 || !(obj[0] is ObstacleData obstacleData)) return;
            
            OnLifeLost();
        }

        private void OnGameStart(object[] obj)
        {
            Reset();
            OnLifeEarned();
        }

        private void Reset()
        {
            LifeCount = default;
        }

        private void OnLifeEarned()
        {
            LifeCount++;
            
            Debug.Log($"[{nameof(PlayerLifeDataManager)}] {nameof(OnLifeEarned)} Life count {LifeCount}");

        }
        
        private void OnLifeLost()
        {
            LifeCount--;
            
            Debug.Log($"[{nameof(PlayerLifeDataManager)}] {nameof(OnLifeLost)} Life count {LifeCount}");
            if(LifeCount <= 0) new EventCommand(GameEvents.Gameplay.Stop).Execute();
        }
    }
    
    internal static partial class UiDataKeys
    {
        internal static partial class Player
        {
            public static readonly string Lives = $"{nameof(Player)}.{nameof(Lives)}";
        }
    }
}