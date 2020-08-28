using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RollyVortex
{
    internal class ObstacleCacheController
    {
        private readonly Transform _cache;

        private readonly Queue<ObstacleWrapper> _cachedObstacles;
        private readonly Queue<ObstacleWrapper> _movingObstacles;

        private readonly Vector3 _reCacheMarker;

        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker)
        {
            _cache = cache;

            var capacity = cache.childCount;

            _cachedObstacles = new Queue<ObstacleWrapper>(capacity);
            _movingObstacles = new Queue<ObstacleWrapper>(capacity);

            _reCacheMarker = reCacheMarker;

            DistanceToTravel = Math.Abs(_cache.position.z - _reCacheMarker.z);

            for (var i = 0; i < capacity; i++) Cache(new ObstacleWrapper(_cache.GetChild(i)));
        }

        public float DistanceToTravel { get; }

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
                MovementUtils.UpdateObstaclePosition(obstacle.Entry, ref newTime, deltaTime, DistanceToTravel,
                    totalTime);
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
            var cacheEntry = new ObstacleWrapper(Object
                .Instantiate(source, sourceTransform.position, sourceTransform.rotation, _cache).transform);
            Debug.Log(
                $"[{nameof(ObstacleMovement)}] Obstacle cache ran out. Adding new object to cache at runtime. Consider expanding cache.");
            Cache(cacheEntry);
        }

        private void Cache(ObstacleWrapper obs)
        {
            obs.Reset();
            _cachedObstacles.Enqueue(obs);
        }

        private void SpawnNextFromCache()
        {
            var next = _cachedObstacles.Dequeue();
            next.Spawn(ObstacleManager.GetNextObstacleData());
            _movingObstacles.Enqueue(next);
        }

        private class ObstacleWrapper
        {
            private readonly List<GameObject> _children;
            private readonly List<Collider> _colliders;

            private readonly GameObject _go;
            private readonly int _managedCount;
            public readonly Transform Entry;

            public ObstacleWrapper(Transform entry)
            {
                Entry = entry;
                _go = entry.gameObject;
                _managedCount = entry.childCount;

                _colliders = new List<Collider>(_managedCount);
                _children = new List<GameObject>(_managedCount);

                foreach (Transform child in entry.transform)
                {
                    _children.Add(child.gameObject);
                    _colliders.Add(child.GetComponent<Collider>());
                }
            }

            public float Timer { get; set; }

            public void Reset()
            {
                Enable(false);
                Timer = 0;
                SetZ(0f);
            }

            public void Spawn(ObstacleData data)
            {
                if (data == null)
                {
                    Reset();
                    return;
                }

                SetData(data);
                Enable(true);
            }
            
            private void SetZ(float z)
            {
                MovementUtils.SetPositionForObstacle(Entry, z);
            }

            private void SetData(ObstacleData data)
            {
                //Set active
                for (var i = 0; i < _managedCount; i++) _children[i].SetActive(data.IsEnabled(i));
                
                //Set rotation
            }

            private void Enable(bool isEnabled)
            {
                _go.SetActive(isEnabled);
                foreach (var collider in _colliders) collider.enabled = isEnabled;
            }
        }
    }
}