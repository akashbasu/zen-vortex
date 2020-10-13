using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class ObstacleMovement : IGameLoopObserver, ICollisionEventObserver, IPostConstructable
    {
        [Dependency] private readonly ISceneReferenceProvider _sceneReferenceProvider;
        
        private ICacheController _cacheController;

        private float _delayTime;
        private float _loopInSeconds;
        private float _releaseObstacleInSeconds;
        
        private LTDescr _spawnTween;
        
        public void PostConstruct(params object[] args)
        {
            if (!_sceneReferenceProvider.TryGetEntry(Tags.ObstacleCache, out var obstacleCache))
            {
                Debug.LogError($"[{nameof(ObstacleMovement)}] Cannot find references");
                return;
            }
            
            _cacheController = new ObstacleCacheController(obstacleCache.transform, Camera.main.transform.position);
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
            _releaseObstacleInSeconds = _loopInSeconds / data.Visibility;
        }

        public void OnGameStart()
        {
            _spawnTween = LeanTween.delayedCall(_releaseObstacleInSeconds,
                    () => _cacheController.SpawnNext(_loopInSeconds)).setRepeat(-1).setDelay(_delayTime - _releaseObstacleInSeconds);
        }
        
        public void OnGameEnd()
        {
            StopMovement();
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

        public void OnCollisionEnter(GameObject other, int pointOfCollision)
        {
            if (!other.tag.Equals(Tags.Ball)) return;

            _cacheController.Current.CollisionStart(pointOfCollision);
            
            TryNotifyCollision(pointOfCollision);
        }
        
        public void OnCollisionStay(GameObject other) { }

        public void OnCollisionExit(GameObject other, int pointOfCollision)
        {
            if (!other.tag.Equals(Tags.Ball)) return;
            
            _cacheController.Current.CollisionComplete(pointOfCollision);

            TryNotifyCollision(pointOfCollision);
        }

        private void TryNotifyCollision(int pointOfCollision)
        {
            if(!_cacheController.Current.HasActionableCollision) return;
            
            Debug.Log($"[{nameof(ObstacleMovement)}] {nameof(OnCollisionExit)} Point of collision {pointOfCollision} FATAL!");
            new EventCommand(GameEvents.Obstacle.Collision, _cacheController.GetActionableData()).Execute();
        }
    }
}