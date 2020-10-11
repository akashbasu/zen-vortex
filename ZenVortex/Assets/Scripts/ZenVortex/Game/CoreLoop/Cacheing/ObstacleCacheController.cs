using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class ObstacleCacheController : CacheController
    {
        [Dependency] private readonly ObstacleDataManager _obstacleDataManager;
        [Dependency] private readonly LevelDataManager _levelDataManager;
        
        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker,
            typeof(ObstacleController))
        {
            Injector.Inject(this);
        }
        
        protected override object[] GetSpawnData() => new object[] {_obstacleDataManager.GetNextObstacleData(), _levelDataManager.CurrentLevelData};
        protected override object[] GetFireData() => new object[] {_obstacleDataManager.GetCurrentGroupColor};
    }
}