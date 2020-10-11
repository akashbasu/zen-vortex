using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PowerupCacheController : CacheController
    {
        [Dependency] private readonly DeterministicRandomProvider _deterministicRandomProvider;
        [Dependency] private readonly PowerupManager _powerupManager;
        [Dependency] private readonly LevelDataProvider _levelDataProvider;

        internal PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker,
            typeof(PowerupController))
        {
            Injector.Inject(this);
        }

        public override void SpawnNext(float timeToTween)
        {
            if(!_deterministicRandomProvider.NextBool(_levelDataProvider.LevelData.PowerupDropProbability)) return;
            
            base.SpawnNext(timeToTween);
        }

        protected override object[] GetSpawnData() => new object[] {_powerupManager.GetNextPowerupData()};
        protected override object[] GetFireData() => null;
    }
}