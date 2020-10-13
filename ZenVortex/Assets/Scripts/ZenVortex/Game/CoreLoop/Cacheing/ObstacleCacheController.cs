using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class ObstacleCacheController : CacheController
    {
        [Dependency] private readonly IObstacleDataManager _obstacleDataManager;
        [Dependency] private readonly ILevelDataManager _levelDataManager;

        private ObstacleData _spawnData;
        
        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker,
            typeof(ObstacleController))
        {
            Injector.ResolveDependencies(this);
        }
        
        public override object[] GetActionableData() => new object[] {_spawnData};
        
        protected override object[] GetSpawnData()
        {
            _spawnData = _obstacleDataManager.GetNextObstacleData();
            return new object[] {_spawnData, _levelDataManager.CurrentLevelData};
        }
        
        protected override object[] GetFireData() => new object[] {_obstacleDataManager.GetCurrentGroupColor};
    }
}