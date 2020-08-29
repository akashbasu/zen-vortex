using System;

namespace RollyVortex
{
    public class ObstacleManager : IInitializable
    {
        private static int _obstacleGroupCount = -1;
        private static int _obstacleId = -1;
        private static IntRangedValue _groupRange;

        private static ShuffleBag _shuffleBag;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);

            onComplete?.Invoke(this);
        }

        public static ObstacleData GetNextObstacleData()
        {
            _obstacleGroupCount--;
            if (_obstacleGroupCount <= 0 || _obstacleId < 0)
            {
                _obstacleId = _shuffleBag.Next();
                
                _obstacleGroupCount = DeterministicRandomProvider.Next(_groupRange);
                _obstacleGroupCount--;
            }

            return ObstacleDataProvider.ObstacleData[_obstacleId];
        }

        private void OnLevelStart(object[] obj)
        {
            _groupRange = LevelDataProvider.LevelData.Grouping;
            _obstacleGroupCount = -1;
            _obstacleId = -1;

            _shuffleBag = new ShuffleBag(ObstacleDataProvider.ObstacleData.Count);
        }
    }
}