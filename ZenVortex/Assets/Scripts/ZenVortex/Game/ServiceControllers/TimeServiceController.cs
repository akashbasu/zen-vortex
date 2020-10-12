using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface ITimeServiceController : IPostConstructable {}
    
    internal class TimeServiceController : ITimeServiceController
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Powerup.OverrideTimeScale, OnTimeScaleOverride);
            _gameEventManager.Subscribe(GameEvents.Gameplay.Reset, OnReset);
        }

        public void Dispose()
        {
            ResetTimeScaleOverride();
            
            _gameEventManager.Unsubscribe(GameEvents.Powerup.OverrideTimeScale, OnTimeScaleOverride);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Reset, OnReset);
        }

        private void OnReset(object[] obj)
        {
            ResetTimeScaleOverride();
        }

        private void OnTimeScaleOverride(object[] args)
        {
            if(args?.Length < 2) return;
            float scaleFactor = (float) args[0], duration = (float) args[1];

            Time.timeScale = scaleFactor;
            LeanTween.delayedCall(duration, () => Time.timeScale = 1);
        }
        
        private void ResetTimeScaleOverride()
        {
            Time.timeScale = 1;
        }
    }
}