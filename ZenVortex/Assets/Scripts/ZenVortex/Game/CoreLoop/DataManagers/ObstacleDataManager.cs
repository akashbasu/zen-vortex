using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IObstacleDataManager
    {
        ObstacleData GetNextObstacleData();
        Color GetCurrentGroupColor { get; }
    }
    
    internal class ObstacleDataManager : BaseResourceDataManager<ObstacleData>, IObstacleDataManager
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly IDeterministicRandomProvider _deterministicRandomProvider;
        [Dependency] private readonly ILevelDataManager _levelDataManager;

        protected override string DataPath => GameConstants.DataPaths.Resources.Obstacles;
        
        private int _groupCount = -1;
        private int _obstacleId = -1;
        private int _groupColorIndex = -1;
        private IntRangedValue _groupRange;

        private ShuffleBag _shuffleBag;

        public override void PostConstruct(params object[] args)
        {
            base.PostConstruct(args);
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
        }

        public override void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            
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

        private void OnGameStart(object[] obj)
        {
            _groupRange = _levelDataManager.CurrentLevelData.Grouping;
            _groupCount = -1;
            _obstacleId = -1;
            _groupColorIndex = -1;

            _shuffleBag = new ShuffleBag(_data.Length);
        }
    }
}