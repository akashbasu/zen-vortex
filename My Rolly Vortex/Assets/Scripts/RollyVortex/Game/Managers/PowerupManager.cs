using System;

namespace RollyVortex
{
    //Setup powerups
    public class PowerupManager : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            onComplete?.Invoke(this);
        }
    }
}