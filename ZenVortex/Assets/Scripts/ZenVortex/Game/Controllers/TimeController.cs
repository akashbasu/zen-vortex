using System;
using UnityEngine;

namespace ZenVortex
{
    internal class TimeController : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.Gameplay.OverrideTimeScale, OnTimeScaleOverride);
            GameEventManager.Subscribe(GameEvents.Gameplay.End, ResetTimeScaleOverride);
            
            onComplete?.Invoke(this);
        }

        ~TimeController()
        {
            Time.timeScale = 1;
            GameEventManager.Unsubscribe(GameEvents.Gameplay.OverrideTimeScale, OnTimeScaleOverride);
            GameEventManager.Unsubscribe(GameEvents.Gameplay.End, ResetTimeScaleOverride);
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