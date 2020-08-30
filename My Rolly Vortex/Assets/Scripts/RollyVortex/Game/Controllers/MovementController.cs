using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    internal class MovementController : MonoBehaviour, IInitializable
    {
        private bool _canMove;
        private Dictionary<string, ILevelMovement> _objectMovementMap;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                ResetLevelValues();

                GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
                GameEventManager.Subscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
                GameEventManager.Subscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
                GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);

                onComplete?.Invoke(this);
                return;
            }

            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }
        
        private void OnDestroy()
        {
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Unsubscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
            GameEventManager.Unsubscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
        }

        private bool GetReferences()
        {
            if (!DirectoryManager.TryGetEntry(Tags.Board, out var tube)) return false;

            var tubeMovement = new TubeMovement(tube);

            if (!DirectoryManager.TryGetEntry(Tags.Ball, out var ball)) return false;

            var ballMovement = new BallMovement(ball);

            if (!DirectoryManager.TryGetEntry(Tags.ObstacleCache, out var obstacleCache)) return false;

            var obstacleMovement = new ObstacleMovement(obstacleCache);
            
            if (!DirectoryManager.TryGetEntry(Tags.PowerupCache, out var powerupCache)) return false;
            
            var powerupMovement = new PowerupMovement(powerupCache);

            _objectMovementMap = new Dictionary<string, ILevelMovement>
            {
                {Tags.Board, tubeMovement},
                {Tags.Ball, ballMovement},
                {Tags.Obstacle, obstacleMovement},
                {Tags.PowerupCache, powerupMovement}
            };

            return !_objectMovementMap.Any(x => x.Key == null || x.Value == null);
        }

        private void ResetLevelValues()
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.Reset();
        }

        private void OnLevelStart(object[] args)
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.SetLevelData(LevelDataProvider.LevelData);
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnLevelStart();
            
            _canMove = true;
        }

        private void CollisionNotifierOnStart(object[] args)
        {
            if (!(args?.Length >= 3)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;
            var pointOfCollision = (int) args[2];

            if (_objectMovementMap.ContainsKey(source.tag))
                _objectMovementMap[source.tag].OnCollisionEnter(collidedWith, pointOfCollision);
            if (_objectMovementMap.ContainsKey(collidedWith.tag))
                _objectMovementMap[collidedWith.tag].OnCollisionEnter(source, pointOfCollision);
        }
    
        private void CollisionNotifierOnEnd(object[] args)
        {
            if (!(args?.Length >= 2)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;
            var pointOfCollision = (int) args[2];

            if (_objectMovementMap.ContainsKey(source.tag))
                _objectMovementMap[source.tag].OnCollisionExit(collidedWith, pointOfCollision);
            if (_objectMovementMap.ContainsKey(collidedWith.tag))
                _objectMovementMap[collidedWith.tag].OnCollisionExit(source, pointOfCollision);
        }

        private void OnLevelStop(object[] args)
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnLevelEnd();
        }
        
        private void FixedUpdate()
        {
            if (!_canMove) return;

            foreach (var goMovement in _objectMovementMap) goMovement.Value.Update(Time.fixedDeltaTime);
        }
    }
}