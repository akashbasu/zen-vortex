using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PowerupCacheController : CacheController
    {
        [Dependency] private readonly IDeterministicRandomProvider _deterministicRandomProvider;
        [Dependency] private readonly IPowerupDataManager _powerupDataManager;
        [Dependency] private readonly ILevelDataManager _levelDataManager;

        private IBasePowerupData _spawnData;

        internal PowerupCacheController(Transform cache, Vector3 reCacheMarker) : base(cache, reCacheMarker,
            typeof(PowerupController))
        {
            Injector.ResolveDependencies(this);
        }

        public override void SpawnNext(float timeToTween)
        {
            if(!_deterministicRandomProvider.NextBool(_levelDataManager.CurrentLevelData.PowerupDropProbability)) return;
            
            base.SpawnNext(timeToTween);
        }
        
        public override object[] GetActionableData() => new object[] {_spawnData};

        protected override object[] GetSpawnData()
        {
            _spawnData = _powerupDataManager.GetNextPowerupData();
            return new object[] {_spawnData};
        }
        
        protected override object[] GetFireData() => null;
    }
}