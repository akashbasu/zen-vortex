using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IScoreDataManager : IPostConstructable {}
    
    internal class ScoreDataManager : IScoreDataManager
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly IUiDataProvider _uiDataProvider;
        
        private int _scoreForRun;
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Subscribe(GameEvents.Gameplay.CrossedObstacle, OnScoredPoint);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.CrossedObstacle, OnScoredPoint);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
        }

        private void OnLevelStart(object[] obj)
        {
            UpdateScore(0);
        }
        
        private void OnScoredPoint(object[] obj)
        {
            var pointsForObstacle = 1;
            if (obj?.Length > 1) pointsForObstacle = (int) obj[0];
            
            UpdateScore(_scoreForRun + pointsForObstacle);
        }

        private void UpdateScore(int newScore)
        {
            _scoreForRun = newScore;
            _uiDataProvider.UpdateData(UiDataKeys.Player.Score, _scoreForRun);
            
            new EventCommand(GameEvents.Gameplay.ScoreUpdated, _scoreForRun).Execute();
        }
    }
    
    internal static partial class UiDataKeys
    {
        internal static partial class Player
        {
            public static readonly string Score = $"{nameof(Player)}.{nameof(Score)}";
        }
    }
}