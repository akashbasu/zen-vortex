using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace RollyVortex
{
    public class InputController : IInitializable
    {
        private UnityInput _input;
        
        private UnityInput.GameActions _gameInput;
        private UnityInput.UIActions _uiActions;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _input = new UnityInput();
            
            _gameInput = _input.Game;
            _uiActions = _input.UI;
            
            _uiActions.Disable();
            // _gameInput.Disable();
            
            GameEventManager.Subscribe(GameEvents.LevelEvents.StartLevel, OnLevelStart);
            GameEventManager.Subscribe(GameEvents.LevelEvents.StopLevel, OnLevelEnd);
            
            onComplete?.Invoke(this);
        }

        private void OnLevelStart(object[] args)
        {
            _uiActions.Disable();
            // _gameInput.Enable();
            
        }
        
        private void OnLevelEnd(object[] args)
        {
            // _gameInput.Disable();
            _uiActions.Enable();
        }
    }

    public class GameInputAdapter
    {
        private bool IsActiveAndEnabled => _gameInput.enabled && IsPointerAvailable;
        private bool IsPointerAvailable => Touch.activeFingers.Count > 0 || Mouse.current.leftButton.isPressed; 
        
        private UnityInput.GameActions _gameInput;
        private readonly float _screenMid = Screen.width / 2f; 
        
        public GameInputAdapter()
        {
            var input = new UnityInput();
            _gameInput = input.Game;
        }

        public void SetGameInputEnabled(bool isEnabled)
        {
            if(isEnabled) _gameInput.Enable();
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