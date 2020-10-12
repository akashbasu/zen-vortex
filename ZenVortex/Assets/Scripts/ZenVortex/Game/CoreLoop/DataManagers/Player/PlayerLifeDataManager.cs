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
            _gameEventManager.Subscribe(GameEvents.Obstacle.Collision, OnLifeLost);
            _gameEventManager.Subscribe(GameEvents.Powerup.EarnedLife, OnLifeEarned);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Unsubscribe(GameEvents.Obstacle.Collision, OnLifeLost);
            _gameEventManager.Unsubscribe(GameEvents.Powerup.EarnedLife, OnLifeEarned);
        }

        private void OnGameStart(object[] obj)
        {
            LifeCount = 1;
        }

        private void OnLifeEarned(object[] obj)
        {
            LifeCount++;
        }
        
        private void OnLifeLost(object[] obj)
        {
            LifeCount--;
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