using System;
using System.Collections.Generic;

namespace ZenVortex.DI
{
    internal static class DependencyRegistry
    {
        private static Dictionary<Type, DependencyData> _registry;
        public static Dictionary<Type, DependencyData> Registry => _registry ?? (_registry = new Dictionary<Type, DependencyData>());

        public static void RegisterInterface<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            AddToRegistry<TInterface, TImplementation>();
        }
        
        public static void RegisterConcreteType<TImplementation>() where TImplementation : class
        {
            AddToRegistry<TImplementation>();
        }

        private static void AddToRegistry<TInterface, TImplementation>() where TImplementation : class
        {
            if(Registry.TryGetValue(typeof(TInterface), out var existingEntry) && existingEntry.instance != null) return;
            
            var newDependency = new DependencyData(typeof(TImplementation));
            
            Registry[typeof(TInterface)] = newDependency;

            Injector.ResolveDependencies(newDependency);
            
            if (newDependency.instance is IPostConstructable postConstructable) postConstructable.PostConstruct();
        }
        
        private static void AddToRegistry<TImplementation>() where TImplementation : class
        {
            if(Registry.TryGetValue(typeof(TImplementation), out var existingEntry) && existingEntry.instance != null) return;
            
            var newDependency = new DependencyData(typeof(TImplementation));

            Registry[typeof(TImplementation)] = newDependency;

            Injector.ResolveDependencies(newDependency);
            
            if (newDependency.instance is IPostConstructable postConstructable) postConstructable.PostConstruct();
        }

        public static void Reset()
        {
            foreach (var registeredDependency in Registry) if (registeredDependency.Value.instance is IPostConstructable postConstructable) postConstructable.Dispose();
        }
    }

    
}