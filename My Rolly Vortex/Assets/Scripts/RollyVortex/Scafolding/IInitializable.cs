using System;

namespace RollyVortex
{
    public interface IInitializable
    {
        void Initialize(Action<IInitializable> onComplete = null, params object[] args);
    }
}