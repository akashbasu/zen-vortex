using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    public class MovementController : MonoBehaviour, IInitializable
    {
        private TubeMovement _tubeMovement;
        private BallMovement _ballMovement;
        
        private bool _canMove;
        
        // private Vector3 _cachedBallPosition;
        private GameObject _tube;
        private GameObject _ball;
        // private Rigidbody _ballRB;

        private Dictionary<GameObject, ILevelMovement> _objectMovementMap;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                ResetLevelValues();
                
                GameEventManager.Subscribe(GameEvents.LevelEvents.StartLevel, OnLevelStart);
                GameEventManager.Subscribe(GameEvents.Gameplay.CollisionStart, OnCollision);
                GameEventManager.Subscribe(GameEvents.LevelEvents.StopLevel, OnLevelStop);
                
                onComplete?.Invoke(this);
                return;
            }
            
            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }

        private bool GetReferences()
        {
            if (!DirectoryManager.TryGetEntry(RollyVortexTags.Board, out _tube)) return false;
            
            _tubeMovement = new TubeMovement(_tube);
            
            if (!DirectoryManager.TryGetEntry(RollyVortexTags.Ball, out _ball)) return false;
            
            _ballMovement = new BallMovement(_ball);

            _objectMovementMap = new Dictionary<GameObject, ILevelMovement>
            {
                {_tube, _tubeMovement}, 
                {_ball, _ballMovement}
            };

            return !_objectMovementMap.Any(x => x.Key == null || x.Value == null);
        }
        
        private void ResetLevelValues()
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.Reset();
        }
        
        private void OnLevelStart(object[] args)
        {
            if (args?.Length >= 1)
            {
                var speed = (float) args[0];
                foreach (var goMovement in _objectMovementMap) goMovement.Value.SetLevelData(speed);
            }
            
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnLevelStart();

            _canMove = true;
        }
        
        private void OnCollision(object[] args)
        {
            if (!(args?.Length >= 2)) return;
            
            var source = args[0] as GameObject;
            var collidedWith = args[1] as GameObject;
                
            if(_objectMovementMap.ContainsKey(source)) _objectMovementMap[source].OnCollisionEnter(collidedWith);
            if(_objectMovementMap.ContainsKey(collidedWith)) _objectMovementMap[collidedWith].OnCollisionEnter(source);
        }
        
        private void OnLevelStop(object[] args)
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnLevelEnd();
        }

        private void Update()
        {
            if(!_canMove) return;

            foreach (var goMovement in _objectMovementMap) goMovement.Value.Update(Time.deltaTime);
        }
    }
}