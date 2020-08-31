using System;

namespace ZenVortex
{
    internal interface IInitializable
    {
        void Initialize(Action<IInitializable> onComplete = null, params object[] args);
    }
}