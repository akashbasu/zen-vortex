using System;
using UnityEngine;

namespace RollyVortex
{
    public abstract class WaitForEventState : IInitializable
    {
        private Action<IInitializable> _onComplete;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            StartWait(onComplete);
        }
        
        protected void StartWait(Action<IInitializable> onComplete)
        {
            _onComplete = onComplete;
            GameEventManager.Subscribe(EndEvent, OnWaitComplete);
        }
        
        protected abstract string EndEvent { get; }

        private void OnWaitComplete(object[] obj)
        {
            GameEventManager.Unsubscribe(EndEvent, OnWaitComplete);
            
            Debug.Log($"[{GetType().Name}] {nameof(OnWaitComplete)}");
            
            _onComplete?.Invoke(this);
            _onComplete = null;
        }
    }

    public class WaitForMetaStartComplete : WaitForEventState, IInitializable
    {
        protected override string EndEvent => GameEvents.Gameplay.Start;
    }
    
    public class WaitForGameLoopEnd : WaitForEventState, IInitializable
    {
        protected override string EndEvent => GameEvents.Gameplay.End;
    }
    
    
}