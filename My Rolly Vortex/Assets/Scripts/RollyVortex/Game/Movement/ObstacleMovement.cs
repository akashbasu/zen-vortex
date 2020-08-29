using UnityEngine;

namespace RollyVortex
{
    internal class ObstacleMovement : ILevelMovement
    {
        private readonly ObstacleCacheController _cacheController;

        private float _delayTime;
        private float _loopInSeconds;
        private float _releaseObstacleInSeconds;
        
        private LTDescr _spawnTween;

        public ObstacleMovement(GameObject obstacleCache)
        {
            _cacheController = new ObstacleCacheController(obstacleCache.transform, Camera.main.transform.position);
        }

       public void Reset()
        {
            StopTween();
            _cacheController.Reset();
        }
        
        public void SetLevelData(LevelData data)
        {
            _delayTime = data.DelayBeforeStart;
            _loopInSeconds = _cacheController.DistanceToTravel / data.ObstacleSpeed;
            _releaseObstacleInSeconds = _loopInSeconds / data.Visibility;
        }

        public void OnLevelStart()
        {
            _spawnTween = LeanTween.delayedCall(_releaseObstacleInSeconds,
                    () => _cacheController.SpawnNext(_loopInSeconds)).setRepeat(-1).setDelay(_delayTime - _releaseObstacleInSeconds);
        }
        
        public void OnLevelEnd()
        {
            StopTween();
        }

        private void StopTween()
        {
            if (_spawnTween == null) return;
            
            LeanTween.cancel(_spawnTween.uniqueId);
            _spawnTween.reset();
            _spawnTween = null;
        }

        public void Update(float deltaTime) { }
        public void OnCollisionEnter(GameObject other) { }
        public void OnCollisionStay(GameObject other) { }
        public void OnCollisionExit(GameObject other) { }
    }
}