using System;
using UnityEngine;

namespace ZenVortex
{
    internal abstract class WaitForEventState : IInitializable
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

    internal class WaitForMetaStartComplete : WaitForEventState
    {
        protected override string EndEvent => GameEvents.Gameplay.Start;
    }
    
    internal class WaitForMetaEndComplete : WaitForEventState
    {
        protected override string EndEvent => GameEvents.Gameplay.Reset;
    }
    
    internal class WaitForGameLoopEnd : WaitForEventState
    {
        protected override string EndEvent => GameEvents.Gameplay.End;
    }
    
    
}