using System;

namespace RollyVortex
{
    public class UiController : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            onComplete?.Invoke(this);
        }
    }
}