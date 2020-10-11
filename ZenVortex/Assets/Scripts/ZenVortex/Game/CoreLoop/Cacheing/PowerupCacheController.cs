using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PowerupCacheController : CacheController
    {
        [Dependency] private readonly DeterministicRandomProvider _deterministicRandomProvider;
        [Dependency] private readonly PowerupDataManager _powerupDataManager;
        [Dependency] private readonly LevelDataManager _levelDataManager;

        internal PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker,
            typeof(PowerupController))
        {
            Injector.Inject(this);
        }

        public override void SpawnNext(float timeToTween)
        {
            if(!_deterministicRandomProvider.NextBool(_levelDataManager.CurrentLevelData.PowerupDropProbability)) return;
            
            base.SpawnNext(timeToTween);
        }

        protected override object[] GetSpawnData() => new object[] {_powerupDataManager.GetNextPowerupData()};
        protected override object[] GetFireData() => null;
    }
}