using UnityEngine;

namespace RollyVortex
{
    internal class ObstacleMovement : ILevelMovement
    {
        private readonly ObstacleCacheController _cacheController;

        private float _clock;
        private float _delayTime;

        private float _loopInSeconds;
        private float _releaseObstacleInSeconds;

        public ObstacleMovement(GameObject obstacleCache)
        {
            _cacheController = new ObstacleCacheController(obstacleCache.transform, Camera.main.transform.position);
        }

        public bool IsEnabled { get; set; }

        public void Reset()
        {
            _clock = 0f;
            _cacheController.Reset();
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

            _cacheController.Update(deltaTime, _loopInSeconds);
            _cacheController.TryRecacheObstacle();

            if (_clock < _releaseObstacleInSeconds) return;

            _cacheController.SpawnNext();
            _clock = 0f;
        }

        public void SetLevelData(LevelData data)
        {
            _delayTime = data.DelayBeforeStart;
            _loopInSeconds = _cacheController.DistanceToTravel / data.ObstacleSpeed;
            _releaseObstacleInSeconds = _loopInSeconds / data.Visibility; //data.ObstacleGroupingDelay;
        }

        public void OnCollisionEnter(GameObject other)
        {
        }

        public void OnCollisionStay(GameObject other)
        {
        }

        public void OnCollisionExit(GameObject other)
        {
        }

        public void OnLevelStart()
        {
        }

        public void OnLevelEnd()
        {
        }
    }
}