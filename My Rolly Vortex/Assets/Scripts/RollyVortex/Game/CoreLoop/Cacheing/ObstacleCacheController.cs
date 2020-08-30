using UnityEngine;

namespace RollyVortex
{
    public sealed class ObstacleCacheController : CacheController
    {
        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker, typeof(ObstacleController)) { }
        protected override object[] GetSpawnData() => new object[] {ObstacleManager.GetNextObstacleData()};
        protected override object[] GetFireData() => new object[] {ObstacleManager.GetCurrentGroupColor};
    }
}