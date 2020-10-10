using UnityEngine;

namespace ZenVortex
{
    internal sealed class ObstacleCacheController : CacheController
    {
        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker, typeof(ObstacleController)) { }
        protected override object[] GetSpawnData() => new object[] {ObstacleManager.GetNextObstacleData(), LevelDataProvider.LevelData};
        protected override object[] GetFireData() => new object[] {ObstacleManager.GetCurrentGroupColor};
    }
}