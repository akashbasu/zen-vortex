using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZenVortex
{
    internal interface ICacheController
    {
        float DistanceToTravel { get; }
        
        void Reset();
        void SpawnNext(float timeForTravel);
        object[] GetActionableData();
        void Pause();
        
        ICacheEntry Current { get; }
    }
    
    internal abstract class CacheController : ICacheController
    {
        private readonly Transform _cache;

        private readonly Type _entryType;
        private readonly Queue<ICacheEntry> _cached;
        private readonly Queue<ICacheEntry> _moving;
        
        public float DistanceToTravel { get; }
        
        public ICacheEntry Current => _moving.Peek();
        
        internal CacheController(Transform cache, Vector3 reCacheMarker, Type entryType)
        {
            _cache = cache;

            var capacity = cache.childCount;

            _cached = new Queue<ICacheEntry>(capacity);
            _moving = new Queue<ICacheEntry>(capacity);

            DistanceToTravel = Math.Abs(_cache.position.z - reCacheMarker.z);

            _entryType = entryType;
            for (var i = 0; i < capacity; i++)
            {
                var instance = Activator.CreateInstance(entryType, new object[]{_cache.GetChild(i)});
                Cache(instance as ICacheEntry);
            }
        }

        public void Pause()
        {
            foreach (var entry in _moving) entry.Pause();
        }

        public void Reset()
        {
            CacheAll();
        }
        
        public virtual void SpawnNext(float timeToTween)
        {
            HandleUnderflow();
            Fire(SpawnNextFromCache(), timeToTween);
        }
        
        public abstract object[] GetActionableData();
        
        private void CacheAll()
        {
            while (_moving.Count > 0) Cache(_moving.Dequeue());
        }
        
        private void Cache(ICacheEntry cacheEntry)
        {
            if(cacheEntry == null) return;
            
            cacheEntry.Reset();
            _cached.Enqueue(cacheEntry);
        }
        
        private void HandleUnderflow()
        {
            if (_cached.Count != 0) return;
            if (_moving.Count == 0) return;

            var source = _moving.Peek().Transform.gameObject;
            var sourceTransform = source.transform;
            var cacheEntry = Activator.CreateInstance(_entryType,
                new object[]{Object.Instantiate(source, sourceTransform.position, sourceTransform.rotation, _cache).transform}) as ICacheEntry;
            Debug.Log(
                $"[{GetType()}]  cache ran out. Adding new object to cache at runtime. Consider expanding cache.");
            Cache(cacheEntry);
        }
        
        private void Fire(ICacheEntry cacheEntry, float time)
        {
            if(cacheEntry == null) return;
            
            _moving.Enqueue(cacheEntry);
            cacheEntry.Fire(DistanceToTravel, time, TryRecache, GetFireData());
        }
        
        private void TryRecache()
        {
            if (_moving.Count == 0) return;
            Cache(_moving.Dequeue());
        }
        
        private ICacheEntry SpawnNextFromCache()
        {
            if (_cached.Count == 0) return null;
            
            var next = _cached.Dequeue();
            next.Spawn(GetSpawnData());
            return next;
        }

        protected abstract object[] GetSpawnData();
        protected abstract object[] GetFireData();
    }
}