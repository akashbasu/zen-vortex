namespace RollyVortex
{
    internal interface IInputAdapter<T>
    {
        void SetEnabled(bool isEnabled);
        bool TryGetInput(out T input);
    }
}