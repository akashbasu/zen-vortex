using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZenVortex.DI
{
    internal class DependencyData
    {
        private readonly Type _implementationType;
            
        public readonly object instance;
        public readonly List<FieldInfo> dependencies;
            
        public DependencyData(Type implementationType)
        {
            _implementationType = implementationType;
            
            instance = CreateInstance();
            dependencies = GetDependencies();
        }
            
        public void ResolveDependency(FieldInfo dependency, object value)
        {
            if (value == null)
            {
                Debug.LogError($"[{nameof(DependencyData)}] {nameof(ResolveDependency)} failed to resolve dependency of {_implementationType} on {dependency.FieldType}.");
                return;
            }
            
            if(dependency.GetValue(instance) != null) return;
                
            dependency.SetValue(instance, value);
            dependencies.Remove(dependency);
        }

        private object CreateInstance()
        {
            if (_implementationType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                return MonoBehaviourUtils.CreateMonoBehaviorSingleton(_implementationType);
            }
                
            return Activator.CreateInstance(_implementationType);
        }

        private List<FieldInfo> GetDependencies()
        {
            return DependencyAttribute.GetUnresolvedDependencies(_implementationType, instance);
        }
    }
}