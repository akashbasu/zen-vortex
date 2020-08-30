using System;

namespace RollyVortex
{
    //Setup score
    public class ScoreManager : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            onComplete?.Invoke(this);
        }
    }
}