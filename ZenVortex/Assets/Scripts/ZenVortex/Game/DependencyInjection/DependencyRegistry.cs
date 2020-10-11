using System;
using System.Collections.Generic;

namespace ZenVortex.DI
{
    internal static class DependencyRegistry
    {
        private static Dictionary<Type, DependencyData> _registry;
        public static Dictionary<Type, DependencyData> Registry => _registry ?? (_registry = new Dictionary<Type, DependencyData>());

        public static void Register<TInterface, TImplementation>() where TImplementation : class, new()
        {
            AddToRegistry<TInterface, TImplementation>();
        }
        
        public static void Register<TImplementation>() where TImplementation : class
        {
            AddToRegistry<TImplementation>();
        }

        private static void AddToRegistry<TInterface, TImplementation>() where TImplementation : class
        {
            if(Registry.TryGetValue(typeof(TInterface), out var existingEntry) && existingEntry.instance != null) return;
            
            Registry[typeof(TInterface)] = new DependencyData(typeof(TImplementation));

            Injector.ResolveDependencies<TInterface>();
            
            if (Registry[typeof(TInterface)].instance is IPostConstructable postConstructable) postConstructable.PostConstruct();
        }
        
        private static void AddToRegistry<TImplementation>() where TImplementation : class
        {
            if(Registry.TryGetValue(typeof(TImplementation), out var existingEntry) && existingEntry.instance != null) return;
            
            Registry[typeof(TImplementation)] = new DependencyData(typeof(TImplementation));

            Injector.ResolveDependencies<TImplementation>();
            
            if (Registry[typeof(TImplementation)].instance is IPostConstructable postConstructable) postConstructable.PostConstruct();
        }
    }

    
}