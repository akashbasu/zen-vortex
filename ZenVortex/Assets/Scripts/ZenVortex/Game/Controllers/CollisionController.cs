using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZenVortex
{
    internal class CollisionController : IInitializable
    {
        private Dictionary<string, ICollisionEventObserver> _objectCollisionMap;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                GameEventManager.Subscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
                GameEventManager.Subscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);

                onComplete?.Invoke(this);
                return;
            }

            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }
        
        ~CollisionController()
        {
            GameEventManager.Unsubscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
            GameEventManager.Unsubscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
        }
        
        private bool GetReferences()
        {
            if (!SceneReferenceProvider.TryGetEntry(Tags.ObstacleCache, out var obstacleCache)) return false;

            var obstacleMovement = new ObstacleMovement(obstacleCache);
            
            if (!SceneReferenceProvider.TryGetEntry(Tags.PowerupCache, out var powerupCache)) return false;
            
            var powerupMovement = new PowerupMovement(powerupCache);

            _objectCollisionMap = new Dictionary<string, ICollisionEventObserver>
            {
                {Tags.Obstacle, obstacleMovement},
                {Tags.Powerup, powerupMovement}
            };

            return !_objectCollisionMap.Any(x => x.Key == null || x.Value == null);
        }
        
        private void CollisionNotifierOnStart(object[] args)
        {
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