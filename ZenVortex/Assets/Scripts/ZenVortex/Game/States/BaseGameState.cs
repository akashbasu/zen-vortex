using System;
using System.Collections.Generic;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal abstract class BaseGameState : IInitializable
    {
        private Action<IInitializable> _callback;
        private Queue<IInitializable> _steps;

        protected BaseGameState()
        {
            InstallDependencies();
        }

        public virtual void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Injector.Inject(this);
            
            _callback = onComplete;

            SetupQueue(GetSteps());

            Debug.Log($"[{GetType()}] {nameof(Initialize)} Step count : {_steps.Count}");

            StartQueue();
        }

        protected virtual void InstallDependencies() {}

        protected virtual Queue<IInitializable> GetSteps() => new Queue<IInitializable>();

        private void SetupQueue(Queue<IInitializable> initializables)
        {
            _steps = initializables;
        }

        private void StartQueue()
        {
            if (!TryCompleteQueue()) InitializeStep();
        }

        private void InitializeStep()
        {
            var step = _steps.Peek();
            Debug.Log($"[{GetType()}] {nameof(InitializeStep)} {step.GetType()}");
            step.Initialize(OnStepComplete);
        }

        private void OnStepComplete(IInitializable completedStep)
        {
            if (_steps.Peek() != completedStep) return;

            Debug.Log($"[{GetType()}] {nameof(OnStepComplete)} {completedStep.GetType()}");

            _steps.Dequeue();
            if (!TryCompleteQueue()) InitializeStep();
        }

        private bool TryCompleteQueue()
        {
            if (_steps.Count != 0) return false;

            Debug.Log($"[{GetType()}] {nameof(OnStepComplete)} Queue complete");

            _callback?.Invoke(this);
            _callback = null;
            return true;
        }
    }
}