using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class ObstacleCacheController : CacheController
    {
        [Dependency] private readonly ObstacleManager _obstacleManager;
        [Dependency] private readonly LevelDataProvider _levelDataProvider;
        
        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker,
            typeof(ObstacleController))
        {
            Injector.Inject(this);
        }
        
        protected override object[] GetSpawnData() => new object[] {_obstacleManager.GetNextObstacleData(), _levelDataProvider.LevelData};
        protected override object[] GetFireData() => new object[] {_obstacleManager.GetCurrentGroupColor};
    }
}