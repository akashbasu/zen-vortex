using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class MovementController : MonoBehaviour, IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        [Dependency] private readonly SceneReferenceProvider _sceneReferenceProvider;
        [Dependency] private readonly LevelDataProvider _levelDataProvider;
        
        private bool _canMove;
        private readonly Dictionary<string, ILevelMovementObserver> _objectMovementMap = new Dictionary<string, ILevelMovementObserver>();

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                ResetLevelValues();

                _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
                _gameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
                _gameEventManager.Subscribe(GameEvents.Gameplay.Reset, OnReset);

                onComplete?.Invoke(this);
                return;
            }

            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }
        
        private void OnDestroy()
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Reset, OnReset);
        }

        private bool GetReferences()
        {
            _objectMovementMap.Clear();
                
            if (!_sceneReferenceProvider.TryGetEntry(Tags.Board, out var tube)) return false;

            var tubeMovement = new TubeMovement(tube);

            if (!_sceneReferenceProvider.TryGetEntry(Tags.Ball, out var ball)) return false;

            var ballMovement = new BallMovement(ball);

            if (!_sceneReferenceProvider.TryGetEntry(Tags.ObstacleCache, out var obstacleCache)) return false;

            var obstacleMovement = new ObstacleMovement(obstacleCache);
            
            if (!_sceneReferenceProvider.TryGetEntry(Tags.PowerupCache, out var powerupCache)) return false;
            
            var powerupMovement = new PowerupMovement(powerupCache);
            
            _objectMovementMap.Add(Tags.Board, tubeMovement);
            _objectMovementMap.Add(Tags.Ball, ballMovement);
            _objectMovementMap.Add(Tags.Obstacle, obstacleMovement);
            _objectMovementMap.Add(Tags.Powerup, powerupMovement);

            return !_objectMovementMap.Any(x => x.Key == null || x.Value == null);
        }

        private void ResetLevelValues()
        {
            _canMove = false;
            foreach (var goMovement in _objectMovementMap) goMovement.Value.Reset();
        }

        private void OnLevelStart(object[] args)
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.SetLevelData(_levelDataProvider.LevelData);
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnLevelStart();
            
            _canMove = true;
        }

        private void OnLevelStop(object[] args)
        {
            _canMove = false;
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnLevelEnd();
        }
        
        private void FixedUpdate()
        {
            if (!_canMove) return;

            foreach (var goMovement in _objectMovementMap) goMovement.Value.Update(Time.fixedDeltaTime);
        }
        
        private void OnReset(object[] args)
        {
            ResetLevelValues();
        }
    }
}