using System;

namespace RollyVortex
{
    public class ObstacleManager : IInitializable
    {
        private static int _obstacleGroupCount = -1;
        private static int _obstacleId = -1;
        private static RangedValue _groupRange;
        private static Random _random;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _random = new Random();

            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);

            onComplete?.Invoke(this);
        }

        public static ObstacleData GetNextObstacleData()
        {
            _obstacleGroupCount--;
            if (_obstacleGroupCount <= 0 || _obstacleId < 0)
            {
                _obstacleId = _random.Next(0, ObstacleDataProvider.ObstacleData.Count - 1);
                _obstacleGroupCount = _random.Next(_groupRange.Min, _groupRange.Max) - 1;
            }

            return ObstacleDataProvider.ObstacleData[_obstacleId];
        }

        private void OnLevelStart(object[] obj)
        {
            _groupRange = LevelDataProvider.LevelData.Grouping;
            _obstacleGroupCount = -1;
            _obstacleId = -1;
        }
    }
}