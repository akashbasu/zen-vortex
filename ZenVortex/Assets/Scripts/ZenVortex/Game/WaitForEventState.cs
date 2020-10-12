using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal abstract class WaitForEventState : IInitializable
    {
        [Dependency] protected readonly IGameEventManager _gameEventManager;
        
        private Action<IInitializable> _onComplete;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Injector.ResolveDependencies(this);
            
            StartWait(onComplete);
        }
        
        protected void StartWait(Action<IInitializable> onComplete)
        {
            _onComplete = onComplete;
            _gameEventManager.Subscribe(EndEvent, OnWaitComplete);
        }
        
        protected abstract string EndEvent { get; }

        private void OnWaitComplete(object[] obj)
        {
            _gameEventManager.Unsubscribe(EndEvent, OnWaitComplete);
            
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
    
    internal class WaitForGameEnd : WaitForEventState
    {
        protected override string EndEvent => GameEvents.Gameplay.Stop;
    }
    
    
}