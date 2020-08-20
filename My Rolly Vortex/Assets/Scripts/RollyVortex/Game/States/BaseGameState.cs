using System;
using System.Collections.Generic;

namespace RollyVortex
{
    public abstract class BaseGameState : IInitializable
    {
        private Action<IInitializable> _callback;
        private Queue<IInitializable> _steps;
        
        public virtual void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _callback = onComplete;
            
            SetupQueue(GetSteps());

            StartQueue();
        }

        protected abstract List<IInitializable> GetSteps();

        protected void SetupQueue(List<IInitializable> initializables)
        {
            _steps = new Queue<IInitializable>();

            foreach (var initializable in initializables) _steps.Enqueue(initializable);
        }
        
        private void StartQueue()
        {
            if (!TryCompleteQueue()) InitializeStep();
        }

        private void InitializeStep()
        {
            var step = _steps.Peek();
            step.Initialize(OnStepComplete);
        }

        private void OnStepComplete(IInitializable completedStep)
        {
            if (_steps.Peek() != completedStep) return;

            _steps.Dequeue();
            if (!TryCompleteQueue()) InitializeStep();
        }

        private bool TryCompleteQueue()
        {
            if (_steps.Count != 0) return false;

            _callback?.Invoke(this);
            _callback = null;
            return true;
        }
    }
}