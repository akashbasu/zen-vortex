using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RollyVortex
{
    internal class ObstacleCacheController
    {
        private readonly Transform _cache;

        private readonly Queue<ObstacleController> _cachedObstacles;
        private readonly Queue<ObstacleController> _movingObstacles;

        public ObstacleCacheController(Transform cache, Vector3 reCacheMarker)
        {
            _cache = cache;

            var capacity = cache.childCount;

            _cachedObstacles = new Queue<ObstacleController>(capacity);
            _movingObstacles = new Queue<ObstacleController>(capacity);

            DistanceToTravel = Math.Abs(_cache.position.z - reCacheMarker.z);

            for (var i = 0; i < capacity; i++) Cache(new ObstacleController(_cache.GetChild(i)));
        }

        public float DistanceToTravel { get; }

        public void Reset()
        {
            CacheAllObstacles();
        }

        public void SpawnNext(float timeToTween)
        {
            HandleUnderflow();
            Fire(SpawnNextFromCache(), timeToTween);
        }

        private void TryRecacheObstacle()
        {
            if (_movingObstacles.Count == 0) return;
            Cache(_movingObstacles.Dequeue());
        }

        private void CacheAllObstacles()
        {
            while (_movingObstacles.Count > 0) Cache(_movingObstacles.Dequeue());
        }

        private void HandleUnderflow()
        {
            if (_cachedObstacles.Count != 0) return;

            var source = _movingObstacles.Peek().Transform.gameObject;
            var sourceTransform = source.transform;
            var cacheEntry = new ObstacleController(Object
                .Instantiate(source, sourceTransform.position, sourceTransform.rotation, _cache).transform);
            Debug.Log(
                $"[{nameof(ObstacleMovement)}] Obstacle cache ran out. Adding new object to cache at runtime. Consider expanding cache.");
            Cache(cacheEntry);
        }

        private void Cache(ObstacleController obs)
        {
            obs.Reset();
            _cachedObstacles.Enqueue(obs);
        }

        private void Fire(ObstacleController obs, float time)
        {
            _movingObstacles.Enqueue(obs);
            obs.Begin(DistanceToTravel, time, TryRecacheObstacle);
        }

        private ObstacleController SpawnNextFromCache()
        {
            var next = _cachedObstacles.Dequeue();
            next.Spawn(ObstacleManager.GetNextObstacleData());
            return next;
        }
    }
}