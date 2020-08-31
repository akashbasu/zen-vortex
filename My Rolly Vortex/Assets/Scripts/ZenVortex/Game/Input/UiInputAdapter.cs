namespace ZenVortex
{
    internal class UiInputAdapter : IInputAdapter<bool>
    {
        private UnityInput.UIActions _uiInput;

        private bool IsActiveAndEnabled => _uiInput.enabled;

        internal UiInputAdapter(UnityInput.UIActions ui)
        {
            _uiInput = ui;
        }
        
        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled) _uiInput.Enable();
            else _uiInput.Disable();
        }

        public bool TryGetInput(out bool input)
        {
            input = IsActiveAndEnabled;
            return input;
        }
    }
}