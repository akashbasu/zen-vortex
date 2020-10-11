using System.Linq;
using UnityEngine;

namespace ZenVortex.DI
{
    internal static class Injector
    {
        public static void Inject(object instance)
        {
            var dependencies = DependencyAttribute.GetUnresolvedDependencies(instance.GetType(), instance);
            
            foreach (var dependency in dependencies)
            {
                if (DependencyRegistry.Registry.ContainsKey(dependency.FieldType)) dependency.SetValue(instance, DependencyRegistry.Registry[dependency.FieldType].instance);
                else Debug.LogError($"Failed to resolve dependency of type {dependency.FieldType} for {instance.GetType()}");
            }
        }

        public static void ResolveDependencies<TDependency>()
        {
            var dependencyInstance = DependencyRegistry.Registry[typeof(TDependency)].instance; 
            Inject(dependencyInstance);
            ResolveExistingDependencies<TDependency>(dependencyInstance);
        }

        private static void ResolveExistingDependencies<TDependency>(object value)
        {
            var unresolvedTypes = DependencyRegistry.Registry.Values.Where(x => x.dependencies.Count > 0);
            foreach (var unresolvedType in unresolvedTypes)
            {
                var resolvables = unresolvedType.dependencies.FindAll(x => x.FieldType == typeof(TDependency));
                foreach (var resolvable in resolvables) unresolvedType.ResolveDependency(resolvable, value);
            }
        }
    }
}