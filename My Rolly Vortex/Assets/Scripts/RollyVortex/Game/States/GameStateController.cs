using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{ 
    public sealed class GameStateController : MonoBehaviour
    {
        private readonly Queue<IInitializable> _steps = new Queue<IInitializable>();

        [SerializeField] private IInitializable[] _initializableMonobehaviorSystemObjects;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            AddGameStates(true);
            NextState();
        }

        private void AddGameStates(bool addOneTimeStates = false)
        {
            _steps.Clear();
            
            if (addOneTimeStates)
            {
                _steps.Enqueue(new BootState());
                _steps.Enqueue(new PlayerState());
            }
            
            _steps.Enqueue(new MetaStartState());
            _steps.Enqueue(new GameState());
            _steps.Enqueue(new MetaEndState());
            _steps.Enqueue(new ResetState());
        }

        private void NextState()
        {
            if(_steps.Count == 0) AddGameStates();
            ProcessState(_steps.Peek());
       }

        private void ProcessState(IInitializable step)
        {
            object[] args = null;
            if(step is BootState) args = _initializableMonobehaviorSystemObjects;
            
            Debug.Log($"[{nameof(GameStateController)}] {nameof(ProcessState)} {step.GetType()}");
            new Command(GameEvents.GameStateEvents.Start, step.GetType()).Execute();
            step.Initialize(OnStepComplete, args);
        }

        private void OnStepComplete(IInitializable initializable)
        {
            if(_steps.Peek() != initializable) return;
            
            _steps.Dequeue();
            Debug.Log($"[{nameof(GameStateController)}] {nameof(OnStepComplete)} Completed {initializable.GetType()}");
            new Command(GameEvents.GameStateEvents.End, initializable.GetType()).Execute();
            NextState();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _initializableMonobehaviorSystemObjects = gameObject.GetComponents<IInitializable>();

            foreach (var initializable in _initializableMonobehaviorSystemObjects)
                if (initializable == null)
                    Debug.LogError(
                        $"[{nameof(GameStateController)}] {nameof(OnValidate)}  Invalid reference to Monobehavior initializable");
        }
#endif
    }
}