using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleController
    {
        private readonly List<MeshRenderer> _renderers;
        private readonly List<Collider> _colliders;
        private readonly HashSet<int> _fatalCollisions;
        private readonly HashSet<int> _currentCollisions;

        private readonly GameObject _go;
        private readonly int _managedCount;
        
        private ObstacleData _spawnData;

        public readonly Transform Transform;
        public bool HasFatalCollision => _currentCollisions.Any(x => _fatalCollisions.Contains(x));

        public ObstacleController(Transform transform)
        {
            Transform = transform;
            _go = transform.gameObject;
            _managedCount = transform.childCount;

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
            LeanTween.cancel(_go);
            
            Transform.localScale = GameConstants.Animation.Obstacle.ResetScaleValue;
            foreach (var renderer in _renderers) renderer.enabled = true;
            _fatalCollisions.Clear();
            _currentCollisions.Clear();
            _go.LeanColor(Color.clear, 0);
                
            Enable(false);
            SetZ(0f);
        }

        public void Spawn(ObstacleData data)
        {
            _spawnData = data;
            if (_spawnData == null)
            {
                Reset();
                return;
            }

            SetSpawnData();
            Enable(true);
        }

        public void Begin(float distanceToTravel, Color groupColor,float time, Action onComplete)
        {
            Animate(GameConstants.Animation.Obstacle.TargetScaleValue, groupColor, time * _spawnData.AnimationTimeNormalization);
            StartMovement(distanceToTravel, time, onComplete);
            StartRotation(time * _spawnData.RotationTimeNormalization);
        }

        public void CollisionStart(int pointOfCollision)
        {
            _currentCollisions.Add(pointOfCollision);
        }
        
        public void CollisionComplete(int pointOfCollision)
        {
            _currentCollisions.Remove(pointOfCollision);
            
            if(_currentCollisions.Count == 0) Animate(GameConstants.Animation.Obstacle.ResetScaleValue, Color.clear, GameConstants.Animation.Obstacle.AnimateOutTime);
        }
        
        private void SetZ(float z)
        {
            MovementUtils.SetPositionForObstacle(Transform, z);
        }

        private void SetSpawnData()
        {
            for (var i = 0; i < _managedCount; i++)
            {
                _renderers[i].enabled = _spawnData.IsEnabled(i);
                if (_renderers[i].enabled) _fatalCollisions.Add(i);
            }
            
            var spawnRotation = DeterministicRandomProvider.Next(_spawnData.SpawnRotation.Min, _spawnData.SpawnRotation.Max);
            MovementUtils.SetRotation(Transform, spawnRotation);
        }
        
        private void StartMovement(float distanceToTravel, float time, Action onComplete)
        {
            Transform.LeanMoveLocalZ(-distanceToTravel, time).setOnComplete(onComplete);
        }
        
        private void StartRotation(float time)
        {
            Transform.LeanRotateZ(DeterministicRandomProvider.Next(_spawnData.TargetRotation.Min, _spawnData.TargetRotation.Max), time)
                .setEase(GameConstants.Animation.Obstacle.Ease);
        }

        private void Animate(Vector3 sizeTarget, Color colorTarget, float time)
        {
            _go.LeanColor(colorTarget, time).setEase(GameConstants.Animation.Obstacle.Ease);
            Transform.LeanScale(sizeTarget, time).setEase(GameConstants.Animation.Obstacle.Ease);
        }

        private void Enable(bool isEnabled)
        {
            _go.SetActive(isEnabled);
            foreach (var collider in _colliders) collider.enabled = isEnabled;
        }
    }
    
    public static partial class GameConstants
    {
        public static partial class Animation
        {
            public static partial class Obstacle
            {
                public static readonly Vector3 ResetScaleValue = new Vector3(2f, 2f, 2f);
                public static readonly Vector3 TargetScaleValue = Vector3.one;
                public const float AnimateOutTime = 0.5f;
                public const LeanTweenType Ease = LeanTweenType.easeInOutSine;
            }
        }
    }
}