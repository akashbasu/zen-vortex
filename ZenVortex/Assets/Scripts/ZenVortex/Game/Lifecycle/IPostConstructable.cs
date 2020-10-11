using System;

namespace ZenVortex
{
    internal interface IPostConstructable : IDisposable
    {
        void PostConstruct(params object[] args);
    }
}