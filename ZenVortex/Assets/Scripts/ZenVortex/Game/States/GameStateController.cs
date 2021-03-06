using System;
using System.Collections.Generic;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{ 
    internal sealed class GameStateController : MonoBehaviour
    {
        private readonly Queue<IInitializable> _steps = new Queue<IInitializable>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            _steps.Clear();
            AddOneTimeStates();
            AddGameStates();
            
            NextState();
        }

        private void OnApplicationQuit()
        {
            DependencyRegistry.Reset();
        }

        private void AddOneTimeStates()
        {
            _steps.Enqueue(new BootState());
            _steps.Enqueue(new PlayerState());
        }

        private void AddGameStates()
        {
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
            Debug.Log($"[{nameof(GameStateController)}] {nameof(ProcessState)} {step.GetType().Name}");
            new EventCommand(GameEvents.StateMachineEvents.Start, step.GetType().Name).Execute();
            step.Initialize(OnStepComplete);
        }

        private void OnStepComplete(IInitializable initializable)
        {
            if(_steps.Peek() != initializable) return;
            
            _steps.Dequeue();
            Debug.Log($"[{nameof(GameStateController)}] {nameof(OnStepComplete)} Completed {initializable.GetType().Name}");
            new EventCommand(GameEvents.StateMachineEvents.End, initializable.GetType().Name).Execute();
            NextState();
        }
    }
}