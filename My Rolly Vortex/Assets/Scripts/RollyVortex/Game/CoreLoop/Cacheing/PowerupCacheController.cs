using UnityEngine;

namespace RollyVortex
{
    public sealed class PowerupCacheController : CacheController
    {
        public PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker, typeof(PowerupController)) { }
        protected override object[] GetSpawnData() => null;
        protected override object[] GetFireData() => null;
    }
}