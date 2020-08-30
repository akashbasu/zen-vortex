using System;
using System.Collections.Generic;

namespace RollyVortex
{
    public class InputController : IInitializable
    {
        private static UnityInput _input;
        
        private static readonly List<string> GameStates = new List<string>()
        {
            nameof(GameState)
        };

        public static GameInputAdapter GameInput { get; private set; }
        public static UiInputAdapter UiInput { get; private set; }

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _input = new UnityInput();

            GameInput = new GameInputAdapter(_input.Game);
            UiInput = new UiInputAdapter(_input.UI);

            UiInput.SetEnabled(false);
            GameInput.SetEnabled(false);

            GameEventManager.Subscribe(GameEvents.GameStateEvents.Start, OnGameStateStart);
            GameEventManager.Subscribe(GameEvents.GameStateEvents.End, OnGameStateEnd);

            onComplete?.Invoke(this);
        }

        ~InputController()
        {
            GameEventManager.Unsubscribe(GameEvents.GameStateEvents.Start, OnGameStateStart);
            GameEventManager.Unsubscribe(GameEvents.GameStateEvents.End, OnGameStateEnd);
        }

        private void OnGameStateStart(object[] obj)
        {
            if (obj?.Length < 1) return;
            
            var currentState = (string) obj[0];

            var isGameState = GameStates.Contains(currentState);
            GameInput.SetEnabled(isGameState);
            UiInput.SetEnabled(!isGameState);
        }
        
        private void OnGameStateEnd(object[] obj)
        {
            GameInput.SetEnabled(false);
            UiInput.SetEnabled(false);
        }
    }
}