using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class TimeController : IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Gameplay.OverrideTimeScale, OnTimeScaleOverride);
            _gameEventManager.Subscribe(GameEvents.Gameplay.End, ResetTimeScaleOverride);
            
            onComplete?.Invoke(this);
        }

        ~TimeController()
        {
            Time.timeScale = 1;
            
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.OverrideTimeScale, OnTimeScaleOverride);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.End, ResetTimeScaleOverride);
        }

        private void OnTimeScaleOverride(object[] args)
        {
            if(args?.Length < 2) return;
            float scaleFactor = (float) args[0], duration = (float) args[1];

            Time.timeScale = scaleFactor;
            LeanTween.delayedCall(duration, () => Time.timeScale = 1);
        }
        
        private void ResetTimeScaleOverride(object[] obj)
        {
            Time.timeScale = 1;
        }
    }
}