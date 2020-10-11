using System.Linq;
using UnityEngine;

namespace ZenVortex.DI
{
    internal static class Injector
    {
        public static void ResolveDependencies(object instance)
        {
            var dependencies = DependencyAttribute.GetUnresolvedDependencies(instance.GetType(), instance);
            
            foreach (var dependency in dependencies)
            {
                if (DependencyRegistry.Registry.ContainsKey(dependency.FieldType)) dependency.SetValue(instance, DependencyRegistry.Registry[dependency.FieldType].instance);
                else Debug.LogError($"Failed to resolve dependency of type {dependency.FieldType} for {instance.GetType()}");
            }
        }

        public static void ResolveDependencies(DependencyData dependencyData)
        {
            ResolveInstanceDependencies(dependencyData);
            ResolveExistingDependencies(dependencyData.instance);
        }

        private static void ResolveInstanceDependencies(DependencyData dependencyData)
        {
            var instance = dependencyData.instance;
            var dependencies = dependencyData.dependencies;

            for (int i = dependencies.Count - 1; i >= 0; i--)
            {
                var dependency = dependencies[i];
                if (DependencyRegistry.Registry.ContainsKey(dependency.FieldType)) dependencyData.ResolveDependency(dependency, DependencyRegistry.Registry[dependency.FieldType].instance);
                else Debug.LogError($"Failed to resolve dependency of type {dependency.FieldType} for {instance.GetType()}");
            }
        }
        
        private static void ResolveExistingDependencies(object instance)
        {
            var unresolvedTypes = DependencyRegistry.Registry.Values.Where(x => x.dependencies.Count > 0);
            foreach (var unresolvedType in unresolvedTypes)
            {
                var resolvables = unresolvedType.dependencies.FindAll(x => x.FieldType == instance.GetType());
                foreach (var resolvable in resolvables) unresolvedType.ResolveDependency(resolvable, instance);
            }
        }
    }
}