using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZenVortex
{
    internal abstract class BaseResourceDataManager<TData> : IInitializable where TData : Object
    {
        protected TData[] _data;
        protected abstract string DataPath { get; }
        
        public virtual void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            var resourceLoader = new ResourcesLoader<TData>(DataPath);
            if (!resourceLoader.TryLoadData(out _data))
            {
                Debug.LogException(new Exception($"[{GetType()}] failed to load data."));
                return;
            }
            
            onComplete?.Invoke(this);
        }
    }
    
    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public partial class Resources
            {
                public static readonly string Obstacles = Path.Combine("Data", "Obstacles");
                public static readonly string Powerups = Path.Combine("Data", "Powerups");
                public static readonly string Levels = Path.Combine("Data", "Levels");
            }
        }
    }
}