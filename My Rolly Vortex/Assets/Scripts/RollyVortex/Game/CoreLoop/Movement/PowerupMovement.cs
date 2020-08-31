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
            _delayTime = data.DelayBeforeStart;
            _loopInSeconds = _cacheController.DistanceToTravel / data.ObstacleSpeed;
            _releasePowerupInSeconds = _loopInSeconds / data.Visibility;
        }

        public void OnCollisionEnter(GameObject other, int pointOfCollision)
        {
            if (!other.tag.Equals(Tags.Ball)) return;

            _cacheController.Current.CollisionStart();
            
            switch (_cacheController.Current.HasActionableCollision)
            {
                case true: Debug.Log($"[{nameof(PowerupMovement)}] {nameof(OnCollisionEnter)} Picked up a powerup!");
                    break;
                case false:
                    break;
            }
        }

        public void OnCollisionExit(GameObject other, int pointOfCollision)
        {
            if (!other.tag.Equals(Tags.Ball)) return;
            
            _cacheController.Current.CollisionComplete();
            
            switch (_cacheController.Current.HasActionableCollision)
            {
                case true: Debug.Log($"[{nameof(PowerupMovement)}] {nameof(OnCollisionExit)} Exiting PU");
                    break;
                case false:
                    
                    break;
            }
        }

        public void OnLevelStart()
        {
            _spawnTween = LeanTween.delayedCall(_releasePowerupInSeconds,
                () => _cacheController.SpawnNext(_loopInSeconds)).setRepeat(-1).setDelay(_delayTime - _releasePowerupInSeconds + (_releasePowerupInSeconds * GameConstants.Animation.Powerup.SpawnTimeOffset));
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
    
    public static partial class GameConstants
    {
        internal static partial class Animation
        {
            internal static partial class Powerup
            {
                public static float SpawnTimeOffset = 0.5f;
            }
        }
    }
}