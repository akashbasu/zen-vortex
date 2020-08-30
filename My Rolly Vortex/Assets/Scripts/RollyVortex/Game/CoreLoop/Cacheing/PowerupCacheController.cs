using UnityEngine;

namespace RollyVortex
{
    internal sealed class PowerupCacheController : CacheController
    {
        internal PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker, typeof(PowerupController)) { }
        protected override object[] GetSpawnData() => null;
        protected override object[] GetFireData() => null;
    }
}