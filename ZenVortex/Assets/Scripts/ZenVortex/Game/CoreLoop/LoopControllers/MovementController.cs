using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IMovementController : IPostConstructable {}
    
    internal class MovementController : MonoBehaviour, IMovementController
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly ILevelDataManager _levelDataManager;
        
        [Dependency] private readonly TubeMovement _tubeMovement;
        [Dependency] private readonly BallMovement _ballMovement;
        [Dependency] private readonly ObstacleMovement _obstacleMovement;
        [Dependency] private readonly PowerupMovement _powerupMovement;
        
        private bool _canMove;
        private readonly Dictionary<string, IGameLoopObserver> _objectMovementMap = new Dictionary<string, IGameLoopObserver>();

        public void PostConstruct(params object[] args)
        {
            if (GetManagedObjects())
            {
                ResetLevelValues();

                _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
                _gameEventManager.Subscribe(GameEvents.Gameplay.Stop, OnGameStop);
                _gameEventManager.Subscribe(GameEvents.Gameplay.Reset, OnReset);
                
                return;
            }

            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Stop, OnGameStop);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Reset, OnReset);
        }

        private bool GetManagedObjects()
        {
            _objectMovementMap.Clear();
            
            _objectMovementMap.Add(Tags.Board, _tubeMovement);
            _objectMovementMap.Add(Tags.Ball, _ballMovement);
            _objectMovementMap.Add(Tags.Obstacle, _obstacleMovement);
            _objectMovementMap.Add(Tags.Powerup, _powerupMovement);

            return !_objectMovementMap.Any(x => x.Key == null || x.Value == null);
        }

        private void ResetLevelValues()
        {
            _canMove = false;
            foreach (var goMovement in _objectMovementMap) goMovement.Value.Reset();
        }

        private void OnGameStart(object[] args)
        {
            foreach (var goMovement in _objectMovementMap) goMovement.Value.SetLevelData(_levelDataManager.CurrentLevelData);
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnGameStart();
            
            _canMove = true;
        }

        private void OnGameStop(object[] args)
        {
            _canMove = false;
            foreach (var goMovement in _objectMovementMap) goMovement.Value.OnGameEnd();
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