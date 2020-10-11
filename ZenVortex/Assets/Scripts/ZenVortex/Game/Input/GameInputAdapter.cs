using UnityEngine;
using UnityEngine.InputSystem;

namespace ZenVortex
{
    internal class GameInputAdapter : IInputAdapter<float>
    {
        private readonly float _screenMid = Screen.width / 2f;

        private UnityInput.GameActions _gameInput;

        public GameInputAdapter(UnityInput.GameActions game)
        {
            _gameInput = game;
        }

        private bool IsActiveAndEnabled => _gameInput.enabled && IsPointerAvailable;

        private bool IsPointerAvailable
        {
            get
            {
#if(UNITY_EDITOR || UNITY_STANDALONE)
                return Mouse.current.leftButton.isPressed;
#elif (UNITY_ANDROID || UNITY_IOS)
                return Touchscreen.current.primaryTouch.isInProgress;
#else
                return false;
#endif
            }
        }

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