using System;
using UnityEngine;

namespace RollyVortex
{
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
    
    public abstract class InitializableSingleton<T> : Singleton<T>, IInitializable where T : new()
    {
        public abstract void Initialize(Action<IInitializable> onComplete = null, params object[] args);
    }
}