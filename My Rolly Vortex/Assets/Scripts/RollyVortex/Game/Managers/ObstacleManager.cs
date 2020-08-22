using System;

namespace RollyVortex
{
    //Setup obstacle data
    public class ObstacleManager : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            onComplete?.Invoke(this);
        }
    }
}