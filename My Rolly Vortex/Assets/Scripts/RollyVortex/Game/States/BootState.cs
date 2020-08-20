using System;
using System.Collections.Generic;

namespace RollyVortex
{
    public sealed class BootState : IInitializable
    {
        private Action<IInitializable> _callback;
        private Queue<IInitializable> _bootSteps;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _callback = onComplete;

            SetupQueue(args);
            StartQueue();
        }

        private void SetupQueue(object[] args)
        {
            _bootSteps = new Queue<IInitializable>();

            _bootSteps.Enqueue(new GameEventManager());
            foreach (var monoBehaviorBootables in args) _bootSteps.Enqueue(monoBehaviorBootables as IInitializable);
            _bootSteps.Enqueue(new UiController());
        }

        private void StartQueue()
        {
            if (!TryCompleteQueue()) InitializeStep();
        }

        private void InitializeStep()
        {
            var step = _bootSteps.Peek();
            step.Initialize(OnStepComplete);
        }

        private void OnStepComplete(IInitializable completedStep)
        {
            if (_bootSteps.Peek() != completedStep) return;

            _bootSteps.Dequeue();
            if (!TryCompleteQueue()) InitializeStep();
        }

        private bool TryCompleteQueue()
        {
            if (_bootSteps.Count != 0) return false;

            _callback?.Invoke(this);
            _callback = null;
            return true;
        }
    }
}