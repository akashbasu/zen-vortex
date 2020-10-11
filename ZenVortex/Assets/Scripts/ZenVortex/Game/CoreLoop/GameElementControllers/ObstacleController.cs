using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class ObstacleController : ICacheEntry
    {
        [Dependency] private readonly IPlayerDataManager _playerDataManager;
        [Dependency] private readonly IDeterministicRandomProvider _deterministicRandomProvider;
        
        private readonly List<MeshRenderer> _renderers;
        private readonly List<Collider> _colliders;
        private readonly HashSet<int> _fatalCollisions;
        private readonly HashSet<int> _currentCollisions;

        private readonly GameObject _go;
        private readonly ObstacleAnimator _animator;
        private readonly int _managedCount;
        
        private bool _canSpin;
        private ObstacleData _obstacleData;
        private LevelData _levelData;
        private bool _didConsumeLife;

        public Transform Transform { get; }
        public bool HasActionableCollision => _currentCollisions.Any(x => _fatalCollisions.Contains(x)) && _playerDataManager.LifeCount <= 0;

        //Runtime injection
        public ObstacleController(Transform transform)
        {
            Injector.ResolveDependencies(this);
            
            Transform = transform;
            _go = transform.gameObject;
            _managedCount = transform.childCount;
            _animator = new ObstacleAnimator(_go);

            _colliders = new List<Collider>(_managedCount);
            _renderers = new  List<MeshRenderer>(_managedCount);
            _fatalCollisions = new HashSet<int>();
            _currentCollisions = new HashSet<int>();

            foreach (Transform child in transform.transform)
            {
                _colliders.Add(child.GetComponent<Collider>());
                _renderers.Add(child.GetComponent<MeshRenderer>());
            }
        }

        public void Reset()
        {
            _animator.Reset();

            _didConsumeLife = false;
            _canSpin = true;
            Transform.localScale = GameConstants.Animation.Obstacle.ResetScaleValue;
            foreach (var renderer in _renderers) renderer.enabled = true;
            _fatalCollisions.Clear();
            _currentCollisions.Clear();
            
            Enable(false);
            SetZ(0f);
        }

        public void Spawn(params object[] args)
        {
            if (args?.Length < 2 || !(args[0] is ObstacleData spawnData) || !(args[1] is LevelData levelData))
            {
                Reset();
                return;
            }

            _obstacleData = spawnData;
            _levelData = levelData;

            ApplySpawnData();
            Enable(true);
        }

        public void Fire(float distanceToTravel, float time, Action onComplete, params object[] args)
        {
            if (args?.Length < 1 || !(args[0] is Color groupColor)) return;
            
            var animationParams = new ObstacleAnimator.AnimateInParams(distanceToTravel, time,
                time * _obstacleData.AnimationInTimeNormalization,
                GameConstants.Animation.Obstacle.TargetScaleValue, GetNextTargetRotation(),
                time * _obstacleData.RotationTimeNormalization(_levelData.Visibility), groupColor);
                
            _animator.AnimateIn(animationParams, onComplete);
        }

        public void Pause()
        {
            _animator.Pause();
        }

        public void CollisionStart(params object[] args)
        {
            if (args?.Length > 0)
            {
                _currentCollisions.Add((int)args[0]);
                _didConsumeLife = _currentCollisions.Any(x => _fatalCollisions.Contains(x));
            }
        }
        
        public void CollisionComplete(params object[] args)
        {
            if (args?.Length > 0)
            {
                _currentCollisions.Remove((int)args[0]);    
            }

            if (_currentCollisions.Count != 0) return;

            if (_didConsumeLife)
            {
                new EventCommand(GameEvents.Gameplay.ConsumeLife).Execute();
            }
            
            new EventCommand(GameEvents.Gameplay.CrossedObstacle).Execute();
            _animator.AnimateOut(new ObstacleAnimator.AnimateOutParams(GameConstants.Animation.Obstacle.ResetScaleValue, GameConstants.Animation.Obstacle.AnimateOutTime));
        }
        
        private void SetZ(float z)
        {
            MovementUtils.SetPositionForObstacle(Transform, z);
        }

        private void ApplySpawnData()
        {
            for (var i = 0; i < _managedCount; i++)
            {
                _renderers[i].enabled = _obstacleData.IsEnabled(i);
                if (_renderers[i].enabled) _fatalCollisions.Add(i);
            }

            _canSpin = _deterministicRandomProvider.NextBool(_levelData.RotationProbability);
            
            MovementUtils.SetRotation(Transform, GetNextSpawnRotation());
        }
        
        private float GetNextSpawnRotation() => _canSpin 
            ? _deterministicRandomProvider.Next(_obstacleData.SpawnRotation) 
            : _deterministicRandomProvider.Next(_obstacleData.TargetRotation);

        private float GetNextTargetRotation() => _canSpin
            ? _deterministicRandomProvider.Next(_obstacleData.TargetRotation)
            : MovementUtils.GetCurrentRotation(Transform);

        private void Enable(bool isEnabled)
        {
            _go.SetActive(isEnabled);
            foreach (var collider in _colliders) collider.enabled = isEnabled;
        }
    }

    public static partial class GameConstants
    {
        internal static partial class Animation
        {
            internal static partial class Obstacle
            {
                public static readonly Vector3 ResetScaleValue = new Vector3(2f, 2f, 2f);
                public static readonly Vector3 TargetScaleValue = Vector3.one;
                public const float AnimateOutTime = 0.5f;
                public const LeanTweenType Ease = LeanTweenType.easeInOutSine;
            }
        }
    }
}