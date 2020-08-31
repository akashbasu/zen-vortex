using UnityEngine;

namespace RollyVortex
{
    internal sealed class PowerupCacheController : CacheController
    {
        internal PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker, typeof(PowerupController)) { }

        public override void SpawnNext(float timeToTween)
        {
            var canSpawn = DeterministicRandomProvider.NextNormalized();
            if(canSpawn > LevelDataProvider.LevelData.PowerupDropProbability) return;
            
            base.SpawnNext(timeToTween);
        }

        protected override object[] GetSpawnData() => new object[] {PowerupManager.GetNextPowerupData()};
        protected override object[] GetFireData() => null;
    }
}