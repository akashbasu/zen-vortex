using UnityEngine;

namespace ZenVortex
{
    internal sealed class PowerupCacheController : CacheController
    {
        internal PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker, typeof(PowerupController)) { }

        public override void SpawnNext(float timeToTween)
        {
            if(!DeterministicRandomProvider.NextBool(LevelDataProvider.LevelData.PowerupDropProbability)) return;
            
            base.SpawnNext(timeToTween);
        }

        protected override object[] GetSpawnData() => new object[] {PowerupManager.GetNextPowerupData()};
        protected override object[] GetFireData() => null;
    }
}