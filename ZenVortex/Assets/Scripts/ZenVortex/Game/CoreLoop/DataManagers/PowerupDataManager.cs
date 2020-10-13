using JetBrains.Annotations;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IPowerupDataManager
    {
        [CanBeNull] IBasePowerupData GetNextPowerupData();
    }
    
    internal class PowerupDataManager : BaseResourceDataManager<BasePowerup>, IPowerupDataManager
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        
        protected override string DataPath => GameConstants.DataPaths.Resources.Powerups;
        
        private ShuffleBag _shuffleBag;
        private int _powerupId = -1;

        public override void PostConstruct(params object[] args)
        {
            base.PostConstruct(args);
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
        }

        public override void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            
            base.Dispose();
        }
        
        public IBasePowerupData GetNextPowerupData()
        {
            _powerupId = _shuffleBag.Next();
            return _powerupId >= _data.Length || _powerupId < 0 ? null : _data[_powerupId];
        }
        
        private void OnGameStart(object[] obj)
        {
            Reset();
        }

        private void Reset()
        {
            _shuffleBag = new ShuffleBag(_data.Length);
        }
    }
}