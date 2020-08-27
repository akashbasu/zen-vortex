using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    internal class ObstacleCacheController
    {
        private class ObstacleMovementWrapper
        {
            public readonly Transform Entry;
            public float Timer { get; set; }

            private readonly GameObject _go;
            private readonly Collider[] _colliders;

            public ObstacleMovementWrapper(Transform entry)
            {
                Entry = entry;
                _go = entry.gameObject;
                _colliders = entry.GetComponentsInChildren<Collider>();
            }

            public void Reset()
            {
                Enable(false);
                Timer = 0;
                SetZ(0f);
            }

            private void SetZ(float z)
            {
                MovementUtils.SetPositionForObstacle(Entry, z);
            }

            public void Spawn()
            {
                Enable(true);
            }

            private void Enable(bool isEnabled)
            {
                _go.SetActive(isEnabled);
                foreach (var collider in _colliders) collider.enabled = isEnabled;
            }
        }
        
        private readonly Queue<ObstacleMovementWrapper> _cachedObstacles;
        private readonly Queue<ObstacleMovementWrapper> _movingObstacles;
        
        private readonly Vector3 _reCacheMarker;
        private readonly Transform _cache;
        private readonly float _distanceToTravel;

         public float DistanceToTravel => _distanceToTravel;

        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker)
        {
            _cache = cache;
            
            var capacity = cache.childCount;
            
            _cachedObstacles = new Queue<ObstacleMovementWrapper>(capacity);
            _movingObstacles = new Queue<ObstacleMovementWrapper>(capacity);

            _reCacheMarker = reCacheMarker;

            _distanceToTravel = Math.Abs(_cache.position.z - _reCacheMarker.z);
            
            for (int i = 0; i < capacity; i++) Cache(new ObstacleMovementWrapper(_cache.GetChild(i)));
        }

        public void Reset()
        {
            CacheAllObstacles();
        }

        public bool TryRecacheObstacle()
        {
            var itemsToReCache = RecacheCount();
            if (itemsToReCache == 0) return false;
            
            while (itemsToReCache > 0) 
            {
                Cache(_movingObstacles.Dequeue());
                itemsToReCache--;
            }
            
            return true;
        }
        
        public void Update(float deltaTime, float totalTime)
        {
            foreach (var obstacle in _movingObstacles)
            {
                var newTime = obstacle.Timer;
                MovementUtils.UpdateObstaclePosition(obstacle.Entry, ref newTime, deltaTime, _distanceToTravel, totalTime);
                obstacle.Timer = newTime;
            }
        }

        public void SpawnNext()
        {
            HandleUnderflow();
            SpawnNextFromCache();
        }

        private void CacheAllObstacles()
        {
            while (_movingObstacles.Count > 0) Cache(_movingObstacles.Dequeue());
        }

        private int RecacheCount()
        {
            return _movingObstacles.Count > 0
                ? _movingObstacles.Count(x => MovementUtils.HasReachedDestination(x.Entry, _reCacheMarker))
                : 0;
        }
        
        private void HandleUnderflow()
        {
            if (_cachedObstacles.Count != 0) return;
            
            var source = _movingObstacles.Peek().Entry.gameObject;
            var sourceTransform = source.transform;
            var cacheEntry = new ObstacleMovementWrapper(GameObject.Instantiate(source, sourceTransform.position, sourceTransform.rotation, _cache).transform);
            Debug.Log($"[{nameof(ObstacleMovement)}] Obstacle cache ran out. Adding new object to cache at runtime. Consider expanding cache.");
            Cache(cacheEntry);
        }

        private void Cache(ObstacleMovementWrapper obs)
        {
            obs.Reset();
            _cachedObstacles.Enqueue(obs);
        }
        
        private void SpawnNextFromCache()
        {
            var next = _cachedObstacles.Dequeue();
            next.Spawn();
            _movingObstacles.Enqueue(next);
        }
    }
}