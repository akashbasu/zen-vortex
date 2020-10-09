using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenVortex
{
    internal abstract class BaseGameState : IInitializable
    {
        private Action<IInitializable> _callback;
        private Queue<IInitializable> _steps;

        private static GameObject _systemObject;

        public virtual void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _callback = onComplete;

            SetupQueue(GetSteps(args));

            Debug.Log($"[{GetType()}] {nameof(Initialize)} Step count : {_steps.Count}");

            StartQueue();
        }

        protected abstract List<IInitializable> GetSteps(object[] args);
        
        protected static T CreateMonoBehavior<T>() where T : Component
        {
            if (_systemObject == null)
            {
                if (!SceneReferenceProvider.TryGetEntry(Tags.System, out _systemObject))
                {
                    Debug.LogWarning("Failed to find system object!");
                    return null;
                }
            }

            var component = _systemObject.GetComponent<T>();
            component = component == null ? _systemObject.AddComponent<T>() : component;
            return component;
        }

        private void SetupQueue(List<IInitializable> initializables)
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