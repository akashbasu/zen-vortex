using System;

namespace RollyVortex
{
    internal class ScoreManager : IInitializable
    {
        private int _scoreForRun;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelEnd);
            
            onComplete?.Invoke(this);
        }

        private void OnLevelStart(object[] obj)
        {
            UpdateScore(0);
            GameEventManager.Subscribe(GameEvents.Gameplay.CrossedObstacle, OnScoredPoint);
        }

        private void OnLevelEnd(object[] obj)
        {
            GameEventManager.Unsubscribe(GameEvents.Gameplay.CrossedObstacle, OnScoredPoint);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelEnd);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
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
            UiDataProvider.UpdateData(UiDataKeys.Player.Score, _scoreForRun.ToString());
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