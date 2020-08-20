using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    enum GameStates
    {
        Invalid = -1,
        Boot = 0,
        Game = 1
    }

    public sealed class GameStateController : MonoBehaviour
    {
        private GameStates _gameState = GameStates.Invalid;
        private readonly Dictionary<GameStates, IInitializable> _gameStates = new Dictionary<GameStates, IInitializable>();
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            AddGameStates();
            ProcessState(_gameState + 1);
        }

        private void AddGameStates()
        {
            _gameStates.Add(GameStates.Invalid, null);
            _gameStates.Add(GameStates.Boot, new BootState());
            _gameStates.Add(GameStates.Game, new GameState());
        }

        private void ProcessState(GameStates state)
        {
            if(state == _gameState) return;

            Debug.Log($"[{nameof(GameStateController)}] Old {_gameState} New {state}");
            
            _gameState = state;
            var initializable = _gameStates[_gameState];
            object[] args = null;
            
            switch (_gameState)
            {
                case GameStates.Boot:
                    args = gameObject.GetComponents<IInitializable>();
                    break;
            }
            
            GameEventManager.Broadcast(GameEvents.GameStateEvents.Start, _gameState);
            initializable.Initialize(OnStepComplete, args);
        }

        private void OnStepComplete(IInitializable initializable)
        {
            GameEventManager.Broadcast(GameEvents.GameStateEvents.End, _gameState);
            
            ProcessState(_gameState + 1);
        }
    }
}