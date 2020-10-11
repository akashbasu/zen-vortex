using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class CollisionController : IPostConstructable
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly ISceneReferenceProvider _sceneReferenceProvider;
        
        private bool _canPropagateCollisions;
        private readonly Dictionary<string, ICollisionEventObserver> _objectCollisionMap = new Dictionary<string, ICollisionEventObserver>();

        public void PostConstruct(params object[] args)
        {
            if (GetReferences())
            {
                _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
                _gameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
                return;
            }

            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }

        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
        }
        
        private bool GetReferences()
        {
            _objectCollisionMap.Clear();
            
            if (!_sceneReferenceProvider.TryGetEntry(Tags.ObstacleCache, out var obstacleCache)) return false;

            var obstacleMovement = new ObstacleMovement(obstacleCache);
            
            if (!_sceneReferenceProvider.TryGetEntry(Tags.PowerupCache, out var powerupCache)) return false;
            
            var powerupMovement = new PowerupMovement(powerupCache);
            
            _objectCollisionMap.Add(Tags.Obstacle, obstacleMovement);
            _objectCollisionMap.Add(Tags.Powerup, powerupMovement);

            return !_objectCollisionMap.Any(x => x.Key == null || x.Value == null);
        }
        
        private void OnLevelStart(object[] obj)
        {
            _canPropagateCollisions = true;
            
            _gameEventManager.Subscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
            _gameEventManager.Subscribe(GameEvents.Collisions.Stay, CollisionNotifierOnStay);
            _gameEventManager.Subscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
        }

        private void OnLevelStop(object[] obj)
        {
            _canPropagateCollisions = false;
            
            _gameEventManager.Unsubscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
            _gameEventManager.Unsubscribe(GameEvents.Collisions.Stay, CollisionNotifierOnStay);
            _gameEventManager.Unsubscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
        }

        private void CollisionNotifierOnStart(object[] args)
        {
            if(!_canPropagateCollisions) return;
            
            if (!(args?.Length >= 3)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;
            var pointOfCollision = (int) args[2];

            if (_objectCollisionMap.ContainsKey(source.tag))
                _objectCollisionMap[source.tag].OnCollisionEnter(collidedWith, pointOfCollision);
            if (_objectCollisionMap.ContainsKey(collidedWith.tag))
                _objectCollisionMap[collidedWith.tag].OnCollisionEnter(source, pointOfCollision);
        }

        private void CollisionNotifierOnStay(object[] args)
        {
            if(!_canPropagateCollisions) return;
            
            if (!(args?.Length >= 3)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;
            var pointOfCollision = (int) args[2];

            if (_objectCollisionMap.ContainsKey(source.tag))
                _objectCollisionMap[source.tag].OnCollisionEnter(collidedWith, pointOfCollision);
            if (_objectCollisionMap.ContainsKey(collidedWith.tag))
                _objectCollisionMap[collidedWith.tag].OnCollisionEnter(source, pointOfCollision);
        }
        
        private void CollisionNotifierOnEnd(object[] args)
        {
            if(!_canPropagateCollisions) return;
            
            if (!(args?.Length >= 2)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;
            var pointOfCollision = (int) args[2];

            if (_objectCollisionMap.ContainsKey(source.tag))
                _objectCollisionMap[source.tag].OnCollisionExit(collidedWith, pointOfCollision);
            if (_objectCollisionMap.ContainsKey(collidedWith.tag))
                _objectCollisionMap[collidedWith.tag].OnCollisionExit(source, pointOfCollision);
        }
    }
}