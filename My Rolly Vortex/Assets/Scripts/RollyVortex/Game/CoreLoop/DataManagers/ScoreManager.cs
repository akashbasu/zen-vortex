using System;

namespace RollyVortex
{
    //Setup score
    internal class ScoreManager : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            onComplete?.Invoke(this);
        }
    }
}