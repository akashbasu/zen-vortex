using UnityEngine;

namespace ZenVortex
{
    internal class ResourcesLoader<TResource> where TResource : Object
    {
        private readonly string _resourcePath;

        public ResourcesLoader(string resourcePath)
        {
            _resourcePath = resourcePath;
        }
        
        public bool TryLoadData(out TResource[] resource)
        {
            if (LoadDataFromDisk(out resource)) return true;

            Debug.LogError($"{GetType().Name} {nameof(TryLoadData)} failed to load data of type <{nameof(TResource)}> from disk! Path {_resourcePath}");
            return false;
        }
        
        private bool LoadDataFromDisk(out TResource[] resource)
        {
            resource =  Resources.LoadAll<TResource>(_resourcePath);
            return resource.Length > 0;
        }
    }
}