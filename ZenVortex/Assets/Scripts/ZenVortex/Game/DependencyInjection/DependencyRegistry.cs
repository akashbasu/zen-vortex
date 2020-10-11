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
            Registry[typeof(TInterface)] = new DependencyData(typeof(TImplementation));

            Injector.ResolveDependencies<TInterface>();
        }
        
        private static void AddToRegistry<TImplementation>() where TImplementation : class
        {
            Registry[typeof(TImplementation)] = new DependencyData(typeof(TImplementation));

            Injector.ResolveDependencies<TImplementation>();
        }
    }

    
}