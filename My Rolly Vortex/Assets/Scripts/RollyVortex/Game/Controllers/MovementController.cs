using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    public class MovementController : MonoBehaviour, IInitializable
    {
        // private GameObject _ball;
        // private BallMovement _ballMovement;

        private bool _canMove;
        // private Rigidbody _ballRB;

        private Dictionary<string, ILevelMovement> _objectMovementMap;

        // private Vector3 _cachedBallPosition;
        // private GameObject _tube;
        // private TubeMovement _tubeMovement;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                ResetLevelValues();

                GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
                GameEventManager.Subscribe(GameEvents.Collisions.Start, CollisionNotifierOnStart);
                GameEventManager.Subscribe(GameEvents.Collisions.Stay, CollisionNotifierOnStay);
                GameEventManager.Subscribe(GameEvents.Collisions.End, CollisionNotifierOnEnd);
                GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);

                onComplete?.Invoke(this);
                return;
            }

            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }

        private bool GetReferences()
        {
            if (!DirectoryManager.TryGetEntry(RollyVortexTags.Board, out var tube)) return false;

            var tubeMovement = new TubeMovement(tube);

            if (!DirectoryManager.TryGetEntry(RollyVortexTags.Ball, out var ball)) return false;

            var ballMovement = new BallMovement(ball);

            if (!DirectoryManager.TryGetEntry(RollyVortexTags.ObstacleCache, out var obstacleCache)) return false;

            var obstacleMovement = new ObstacleMovement(obstacleCache); 
            
            _objectMovementMap = new Dictionary<string, ILevelMovement>
            {
                {RollyVortexTags.Board, tubeMovement},
                {RollyVortexTags.Ball, ballMovement},
                {RollyVortexTags.Obstacle, obstacleMovement}
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
            if (!(args?.Length >= 2)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;

            if (_objectMovementMap.ContainsKey(source.tag)) _objectMovementMap[source.tag].OnCollisionEnter(collidedWith);
            if (_objectMovementMap.ContainsKey(collidedWith.tag)) _objectMovementMap[collidedWith.tag].OnCollisionEnter(source);
        }

        private void CollisionNotifierOnStay(object[] args)
        {
            if (!(args?.Length >= 2)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;

            if (_objectMovementMap.ContainsKey(source.tag)) _objectMovementMap[source.tag].OnCollisionStay(collidedWith);
            if (_objectMovementMap.ContainsKey(collidedWith.tag)) _objectMovementMap[collidedWith.tag].OnCollisionStay(source);
        }

        private void CollisionNotifierOnEnd(object[] args)
        {
            if (!(args?.Length >= 2)) return;

            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;

            if (_objectMovementMap.ContainsKey(source.tag)) _objectMovementMap[source.tag].OnCollisionExit(collidedWith);
            if (_objectMovementMap.ContainsKey(collidedWith.tag)) _objectMovementMap[collidedWith.tag].OnCollisionExit(source);
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