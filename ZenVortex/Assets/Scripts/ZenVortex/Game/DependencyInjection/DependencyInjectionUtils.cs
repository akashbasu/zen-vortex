using System;
using UnityEngine;

namespace ZenVortex
{
    internal static class DependencyInjectionUtils
    {
        private static GameObject _systemObject;
        
        public static object CreateMonoBehaviorSingleton(Type implementationType)
        {
            if (_systemObject == null)
            {
                if (MonoBehaviourUtils.SafeGetGoWithTag(Tags.System, out var systemObject))
                {
                    _systemObject = systemObject;
                }
            }

            if (_systemObject != null)
            {
                var component = _systemObject.GetComponent(implementationType);
                component = component == null ? _systemObject.AddComponent(implementationType) : component;
                return component;
            }
            
            Debug.LogWarning("Failed to find system object!");
            return null;
        }
    }
}