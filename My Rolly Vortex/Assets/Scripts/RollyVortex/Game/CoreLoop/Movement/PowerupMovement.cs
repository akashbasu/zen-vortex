using UnityEngine;

namespace RollyVortex
{
    internal class PowerupMovement : ILevelMovement
    {
        private readonly ICacheController _cacheController;
        
        private float _delayTime;
        private float _loopInSeconds;
        private float _releasePowerupInSeconds;
        
        private LTDescr _spawnTween;
        
        internal PowerupMovement(GameObject powerupCache)
        {
            _cacheController = new PowerupCacheController(powerupCache.transform, Camera.main.transform.position);
        }
        
        public void Reset()
        {
            StopTween();
            _cacheController.Reset();
        }

        public void SetLevelData(LevelData data)
        {
            //Set PU data here
        }

        public void OnCollisionEnter(GameObject other, int pointOfCollision)
        {
            //Pick up PU here
        }

        public void OnCollisionExit(GameObject other, int pointOfCollision)
        {
            //Deploy PU here
        }

        public void OnLevelStart()
        {
            _spawnTween = LeanTween.delayedCall(_releasePowerupInSeconds,
                () => _cacheController.SpawnNext(_loopInSeconds)).setRepeat(-1).setDelay(_delayTime - _releasePowerupInSeconds);
        }

        public void OnLevelEnd()
        {
            Reset();
        }
        
        private void StopTween()
        {
            if (_spawnTween == null) return;
            
            LeanTween.cancel(_spawnTween.uniqueId);
            _spawnTween.reset();
            _spawnTween = null;
        }
        
        public void Update(float deltaTime) { }
        public void OnCollisionStay(GameObject other) { }
    }
}