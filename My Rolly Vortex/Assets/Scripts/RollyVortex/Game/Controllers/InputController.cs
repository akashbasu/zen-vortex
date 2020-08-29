using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace RollyVortex
{
    public class InputController : IInitializable
    {
        private UnityInput _input;
        private UnityInput.UIActions _uiActions;

        public static GameInputAdapter GameInput { get; private set; }

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _input = new UnityInput();

            GameInput = new GameInputAdapter(_input.Game);
            _uiActions = _input.UI;

            _uiActions.Disable();
            GameInput.SetEnabled(false);

            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelEnd);

            onComplete?.Invoke(this);
        }

        ~InputController()
        {
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelEnd);
        }

        private void OnLevelStart(object[] args)
        {
            _uiActions.Disable();
            GameInput.SetEnabled(true);
        }

        private void OnLevelEnd(object[] args)
        {
            GameInput.SetEnabled(false);
            _uiActions.Enable();
        }
    }

    public class GameInputAdapter
    {
        private readonly float _screenMid = Screen.width / 2f;

        private UnityInput.GameActions _gameInput;

        public GameInputAdapter(UnityInput.GameActions game)
        {
            _gameInput = game;
        }

        private bool IsActiveAndEnabled => _gameInput.enabled && IsPointerAvailable;
        private static bool IsPointerAvailable => Touch.activeFingers.Count > 0 || Mouse.current.leftButton.isPressed;

        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled) _gameInput.Enable();
            else _gameInput.Disable();
        }

        public bool TryGetInput(out float normalizedInput)
        {
            normalizedInput = 0f;
            if (!IsActiveAndEnabled) return false;

            normalizedInput = _gameInput.Move.ReadValue<Vector2>().x;
            normalizedInput = (normalizedInput - _screenMid) / _screenMid;
            normalizedInput = Mathf.Clamp(normalizedInput, -1f, 1f);
            return true;
        }
    }
}