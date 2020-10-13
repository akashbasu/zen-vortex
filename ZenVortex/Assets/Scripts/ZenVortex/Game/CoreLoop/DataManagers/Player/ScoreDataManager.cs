using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IScoreDataManager : IPostConstructable
    {
        RunScore CurrentScore { get; }
        int HighestScore { get; }
    }
    
    internal class ScoreDataManager : IScoreDataManager
    {
        public RunScore CurrentScore => _currentRunScore;
        public int HighestScore => _highestRunScore.TotalScore;
        
        [Dependency] private readonly IGameEventManager _gameEventManager;

        private RunScore _currentRunScore = new RunScore();
        private readonly RunScore _highestRunScore = new RunScore();
        
        public void PostConstruct(params object[] args)
        {
            _highestRunScore.LoadHighScoreFromDisk(GameConstants.PlayerData.HighScore);
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Subscribe(GameEvents.Obstacle.Crossed, OnCrossedObstacle);
            _gameEventManager.Subscribe(GameEvents.Powerup.Pickup, OnPowerupPickup);
            _gameEventManager.Subscribe(GameEvents.Gameplay.Stop, OnLevelStop);
        }

        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Unsubscribe(GameEvents.Obstacle.Crossed, OnCrossedObstacle);
            _gameEventManager.Unsubscribe(GameEvents.Powerup.Pickup, OnPowerupPickup);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Stop, OnLevelStop);
        }

        private void OnGameStart(object[] obj)
        {
            _currentRunScore = new RunScore();
            
            OnScoreUpdated();
        }
        
        private void OnLevelStop(object[] obj)
        {
            if (_highestRunScore.TotalScore <= _currentRunScore.TotalScore)
            {
                new HighScoreCommand().Execute();
            }
            
            _highestRunScore.SaveHighScoreToDisk(GameConstants.PlayerData.HighScore);
        }
        
        private void OnCrossedObstacle(object[] obj)
        {
            var pointsForObstacle = 1;
            if (obj?.Length >= 1 && obj[0] is ObstacleData obstacleData)
            {
                pointsForObstacle = Math.Max(obstacleData.Points, pointsForObstacle);
            }
            
            _currentRunScore.ObstaclesPassed++;
            _currentRunScore.ObstacleScore += pointsForObstacle;
            
            Debug.Log($"[{nameof(ScoreDataManager)}] {nameof(OnCrossedObstacle)} Obstacles passed : {_currentRunScore.ObstaclesPassed} Score {_currentRunScore.ObstacleScore}");
            
            OnScoreUpdated();
        }
        
        private void OnPowerupPickup(object[] obj)
        {
            var pointsForPowerup = 1;
            if (obj?.Length >= 1 && obj[0] is IBasePowerupData powerupData)
            {
                pointsForPowerup = Math.Max(powerupData.Points, pointsForPowerup);
            }

            _currentRunScore.PowerupsPickedUp++;
            _currentRunScore.PowerupScore += pointsForPowerup;
            
            OnScoreUpdated();
        }

        private void OnScoreUpdated()
        {
            _currentRunScore.TotalScore = GetTotalScore;
            
            _highestRunScore.UpdateMaxScore(_currentRunScore);
            
            new ScoreUpdatedCommand().Execute();
        }
        
        private int GetTotalScore => _currentRunScore.ObstacleScore + _currentRunScore.PowerupScore;
    }
    
    public static partial class GameConstants
    {
        internal static partial class PlayerData
        {
            public static readonly string HighScore = $"{nameof(PlayerData)}.{nameof(HighScore)}.{0}";
        }
    }
    
    internal static partial class UiDataKeys
    {
        internal static partial class Player
        {
            public static readonly string RunScore = $"{nameof(Player)}.{nameof(RunScore)}";
            public static readonly string HighScore = $"{nameof(Player)}.{nameof(HighScore)}";
        }
    }
}