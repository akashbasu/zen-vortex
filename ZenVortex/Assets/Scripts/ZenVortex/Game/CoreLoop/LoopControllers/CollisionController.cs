using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface ICollisionController : IPostConstructable {}
    
    internal class CollisionController : ICollisionController
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        
        [Dependency] private readonly ObstacleMovement _obstacleMovement;
        [Dependency] private readonly PowerupMovement _powerupMovement;
        
        private bool _canPropagateCollisions;
        private readonly Dictionary<string, ICollisionEventObserver> _objectCollisionMap = new Dictionary<string, ICollisionEventObserver>();

        public void PostConstruct(params object[] args)
        {
            if (GetManagedObjects())
            {
                _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
                _gameEventManager.Subscribe(GameEvents.Gameplay.Stop, OnGameStop);
                return;
            }

            Debug.LogError($"[{nameof(CollisionController)}] Cannot find references");
        }

        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Stop, OnGameStop);
        }
        
        private bool GetManagedObjects()
        {
            _objectCollisionMap.Clear();
            
            _objectCollisionMap.Add(Tags.Obstacle, _obstacleMovement);
            _objectCollisionMap.Add(Tags.Powerup, _powerupMovement);

            return !_objectCollisionMap.Any(x => x.Key == null || x.Value == null);
        }
        
        private void OnGameStart(object[] obj)
        {
            _canPropagateCollisions = true;
            
            _gameEventManager.Subscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
            _gameEventManager.Subscribe(GameEvents.Collisions.Stay, CollisionNotifierOnStay);
            _gameEventManager.Subscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
        }

        private void OnGameStop(object[] obj)
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