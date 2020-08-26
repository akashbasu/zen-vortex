using UnityEngine;

namespace RollyVortex
{
    internal class ObstacleMovement : ILevelMovement
    {
        private readonly ObstacleCacheManager _obstacleCache;
        
        private float _loopInSeconds;
        private float _releaseObstacleInSeconds;
        
        private float _clock;
        private float _delayTime;

        public bool IsEnabled { get; set; }
        
        public ObstacleMovement(GameObject obstacleCache)
        {
            _obstacleCache = new ObstacleCacheManager(obstacleCache.transform, Camera.main.transform.position);
        }
        
        public void Reset()
        {
            _clock = 0f;
            _obstacleCache.Reset();
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled)
            {
                if (_clock < _delayTime)
                {
                    _clock += deltaTime;
                    return;
                }

                IsEnabled = true;
                _clock = _releaseObstacleInSeconds;
            }

            _clock += deltaTime;
            

            _obstacleCache.Update(deltaTime, _loopInSeconds);
            _obstacleCache.TryRecacheObstacle();

            if (_clock < _releaseObstacleInSeconds) return;

            _obstacleCache.SpawnNext();
            _clock = 0f;
        }

        public void SetLevelData(LevelData data)
        {
            _delayTime = data.DelayBeforeStart;
            _loopInSeconds = _obstacleCache.DistanceToTravel / data.ObstacleSpeed;
            _releaseObstacleInSeconds = data.ObstacleGroupingDelay;
        }

        public void OnCollisionEnter(GameObject other)
        {
            if (other.tag.Equals(RollyVortexTags.Ball))
            {
                Debug.Log($"[{nameof(ObstacleMovement)}] Ball crashed into obstacle! End level");
            }
        }

        public void OnCollisionStay(GameObject other) { }

        public void OnCollisionExit(GameObject other)
        {
            if (other.tag.Equals(RollyVortexTags.Ball))
            {
                Debug.Log($"[{nameof(ObstacleMovement)}] Ball passed obstacle. Can spawn next.");
            }
        }

        public void OnLevelStart() { }

        public void OnLevelEnd() {}
    }
}