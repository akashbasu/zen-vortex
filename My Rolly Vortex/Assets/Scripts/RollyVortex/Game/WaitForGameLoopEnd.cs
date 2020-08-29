using System;
using UnityEngine;

namespace RollyVortex
{
    public class WaitForGameLoopEnd : IInitializable
    {
        private Action<IInitializable> _onComplete;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _onComplete = onComplete;
            GameEventManager.Subscribe(GameEvents.Gameplay.End, OnGameLoopEnd);
        }

        private void OnGameLoopEnd(object[] obj)
        {
            GameEventManager.Unsubscribe(GameEvents.Gameplay.End, OnGameLoopEnd);
            
            Debug.Log($"[{nameof(WaitForGameLoopEnd)}] {nameof(OnGameLoopEnd)}");
            
            _onComplete?.Invoke(this);
            _onComplete = null;
        }
    }
}