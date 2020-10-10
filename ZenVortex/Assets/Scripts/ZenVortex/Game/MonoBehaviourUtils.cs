using System;
using UnityEngine;

namespace ZenVortex
{
    internal class MonoBehaviourUtils
    {
        public static T CreateMonoBehaviorSingleton<T>() where T : Component
        {
            if (SceneReferenceProvider.TryGetEntry(Tags.System, out var systemObject))
            {
                var component = systemObject.GetComponent<T>();
                component = component == null ? systemObject.AddComponent<T>() : component;
                return component;
            }

            Debug.LogWarning("Failed to find system object!");
            return null;
        }

        public static object CreateMonoBehaviorSingleton(Type implementationType)
        {
            if (SceneReferenceProvider.TryGetEntry(Tags.System, out var systemObject))
            {
                var component = systemObject.GetComponent(implementationType);
                component = component == null ? systemObject.AddComponent(implementationType) : component;
                return component;
            }
            
            Debug.LogWarning("Failed to find system object!");
            return null;
        }
    }
}