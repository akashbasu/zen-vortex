using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    internal enum GameStates
    {
        Invalid = -1,
        Boot = 0,
        Player,
        MetaStart,
        Game,
        MetaEnd,
        End,

        BootComplete = MetaStart,
        GameComplete = End
    }

    public sealed class GameStateController : MonoBehaviour
    {
        private readonly Dictionary<GameStates, IInitializable> _gameStates =
            new Dictionary<GameStates, IInitializable>();

        private GameStates _gameState = GameStates.Invalid;

        [SerializeField] private IInitializable[] _initializableMonobehaviorSystemObjects;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            AddGameStates();
            NextState();
        }

        private void AddGameStates()
        {
            _gameStates.Add(GameStates.Boot, new BootState());
            _gameStates.Add(GameStates.Player, new PlayerState());
            _gameStates.Add(GameStates.MetaStart, new MetaStartState());
            _gameStates.Add(GameStates.Game, new GameState());
            _gameStates.Add(GameStates.MetaEnd, new MetaEndState());
            _gameStates.Add(GameStates.End, new ResetState());
        }

        private void NextState()
        {
            var nextState = _gameState + 1;
            nextState = nextState > GameStates.GameComplete ? GameStates.BootComplete : nextState;
            ProcessState(nextState);
        }

        private void ProcessState(GameStates state)
        {
            if (state == _gameState) return;

            Debug.Log($"[{nameof(GameStateController)}] {nameof(ProcessState)} Old {_gameState} New {state}");

            _gameState = state;
            var initializable = _gameStates[_gameState];
            object[] args = null;

            switch (_gameState)
            {
                case GameStates.Boot:
                    
                    args = _initializableMonobehaviorSystemObjects;
                    break;
            }

            new Command(GameEvents.GameStateEvents.Start, _gameState).Execute();
            initializable.Initialize(OnStepComplete, args);
        }


        private void OnStepComplete(IInitializable initializable)
        {
            Debug.Log($"[{nameof(GameStateController)}] {nameof(OnStepComplete)} Completed {initializable.GetType()}");
            
            new Command(GameEvents.GameStateEvents.End, _gameState).Execute();

            NextState();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _initializableMonobehaviorSystemObjects = gameObject.GetComponents<IInitializable>();
            
            foreach (var initializable in _initializableMonobehaviorSystemObjects)
            {
                if (initializable == null)
                {
                    Debug.LogError($"[{nameof(GameStateController)}] {nameof(OnValidate)}  Invalid reference to Monobehavior initializable");
                }
            }
        }
#endif
    }
}