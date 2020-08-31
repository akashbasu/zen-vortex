using System;
using UnityEngine;

namespace ZenVortex
{
    internal class ObstacleManager : IInitializable
    {
        private static int _groupCount = -1;
        private static int _obstacleId = -1;
        private static int _groupColorIndex = -1;
        private static IntRangedValue _groupRange;

        private static ShuffleBag _shuffleBag;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);

            onComplete?.Invoke(this);
        }

        public static ObstacleData GetNextObstacleData()
        {
            _groupCount--;
            if (_groupCount <= 0 || _obstacleId < 0)
            {
                _obstacleId = _shuffleBag.Next();
                _groupColorIndex = DeterministicRandomProvider.Next(0, GameConstants.Animation.Obstacle.DefaultColors.Count);
                
                _groupCount = DeterministicRandomProvider.Next(_groupRange);
                _groupCount--;
            }

            return ObstacleDataProvider.ObstacleData[_obstacleId];
        }

        public static Color GetCurrentGroupColor => GameConstants.Animation.Obstacle.DefaultColors[_groupColorIndex];

        private void OnLevelStart(object[] obj)
        {
            _groupRange = LevelDataProvider.LevelData.Grouping;
            _groupCount = -1;
            _obstacleId = -1;
            _groupColorIndex = -1;

            _shuffleBag = new ShuffleBag(ObstacleDataProvider.ObstacleData.Count);
        }
        
        private void OnLevelStop(object[] obj)
        {
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
        }
    }
}