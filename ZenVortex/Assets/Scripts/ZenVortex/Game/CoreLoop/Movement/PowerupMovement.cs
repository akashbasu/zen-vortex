using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class PowerupMovement : IGameLoopObserver, ICollisionEventObserver, IPostConstructable
    {
        [Dependency] private readonly ISceneReferenceProvider _sceneReferenceProvider;
        
        private ICacheController _cacheController;
        
        private float _delayTime;
        private float _loopInSeconds;
        private float _releasePowerupInSeconds;
        
        private LTDescr _spawnTween;

        public void PostConstruct(params object[] args)
        {
            if (!_sceneReferenceProvider.TryGetEntry(Tags.PowerupCache, out var powerupCache))
            {
                Debug.LogError($"[{nameof(PowerupMovement)}] Cannot find references");
                return;
            }
            
            _cacheController = new PowerupCacheController(powerupCache.transform, Camera.main.transform.position);
            Reset();
        }
        
        public void Dispose()
        {
            Reset();
        }
        
        public void Reset()
        {
            ResetTween();
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
            
            TryNotifyPickup();
        }

        public void OnCollisionExit(GameObject other, int pointOfCollision)
        {
            if (!other.tag.Equals(Tags.Ball)) return;
            
            _cacheController.Current.CollisionComplete();
        }
        
        public void OnGameStart()
        {
            _spawnTween = LeanTween.delayedCall(_releasePowerupInSeconds,
                () => _cacheController.SpawnNext(_loopInSeconds)).setRepeat(-1).setDelay(_delayTime - _releasePowerupInSeconds + _releasePowerupInSeconds * GameConstants.Animation.Powerup.SpawnTimeOffset);
        }

        public void OnGameEnd()
        {
            StopMovement();
        }

        private void TryNotifyPickup()
        {
            if(!_cacheController.Current.HasActionableCollision) return;
            
            Debug.Log($"[{nameof(PowerupMovement)}] {nameof(TryNotifyPickup)} Picked up powerup!");
            new EventCommand(GameEvents.Powerup.Pickup, _cacheController.GetActionableData()).Execute();
        }

        private void StopMovement()
        {
            _spawnTween?.pause();
            _cacheController.Pause();
        }

        private void ResetTween()
        {
            if (_spawnTween == null) return;
            
            StopMovement();
            
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
                public const float SpawnTimeOffset = 0.5f;
            }
        }
    }
}