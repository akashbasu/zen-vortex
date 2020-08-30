using System;

namespace RollyVortex
{
    internal interface IInitializable
    {
        void Initialize(Action<IInitializable> onComplete = null, params object[] args);
    }
}