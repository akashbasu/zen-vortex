using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class ScoreDataManager : IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        [Dependency] private readonly UiDataProvider _uiDataProvider;
        
        private int _scoreForRun;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Subscribe(GameEvents.Gameplay.CrossedObstacle, OnScoredPoint);
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelEnd);
            
            onComplete?.Invoke(this);
        }

        private void OnLevelStart(object[] obj)
        {
            UpdateScore(0);
        }

        private void OnLevelEnd(object[] obj)
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.CrossedObstacle, OnScoredPoint);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelEnd);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
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