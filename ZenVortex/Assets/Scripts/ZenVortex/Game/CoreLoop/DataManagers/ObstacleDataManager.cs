using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class ObstacleDataManager : BaseResourceDataManager<ObstacleData>
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        [Dependency] private readonly DeterministicRandomProvider _deterministicRandomProvider;
        [Dependency] private readonly LevelDataManager _levelDataManager;

        protected override string DataPath => GameConstants.DataPaths.Resources.Obstacles;
        
        private int _groupCount = -1;
        private int _obstacleId = -1;
        private int _groupColorIndex = -1;
        private IntRangedValue _groupRange;

        private ShuffleBag _shuffleBag;

        public override void PostConstruct(params object[] args)
        {
            base.PostConstruct(args);
            
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
        }

        public override void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            
            base.Dispose();
        }

        public ObstacleData GetNextObstacleData()
        {
            _groupCount--;
            if (_groupCount <= 0 || _obstacleId < 0)
            {
                _obstacleId = _shuffleBag.Next();
                _groupColorIndex = _deterministicRandomProvider.Next(0, GameConstants.Animation.Obstacle.DefaultColors.Count);
                
                _groupCount = _deterministicRandomProvider.Next(_groupRange);
                _groupCount--;
            }

            return _data[_obstacleId];
        }

        public Color GetCurrentGroupColor => GameConstants.Animation.Obstacle.DefaultColors[_groupColorIndex];

        private void OnLevelStart(object[] obj)
        {
            _groupRange = _levelDataManager.CurrentLevelData.Grouping;
            _groupCount = -1;
            _obstacleId = -1;
            _groupColorIndex = -1;

            _shuffleBag = new ShuffleBag(_data.Length);
        }
    }
}