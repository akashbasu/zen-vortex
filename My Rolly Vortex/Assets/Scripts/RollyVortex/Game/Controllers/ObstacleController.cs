using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleController
    {
        private readonly List<GameObject> _children;
        private readonly List<MeshRenderer> _renderers;
        private readonly List<Collider> _colliders;

        private readonly GameObject _go;
        private readonly int _managedCount;
        
        private ObstacleData _spawnData;

        public readonly Transform Transform;

        public ObstacleController(Transform transform)
        {
            Transform = transform;
            _go = transform.gameObject;
            _managedCount = transform.childCount;

            _colliders = new List<Collider>(_managedCount);
            _renderers = new  List<MeshRenderer>(_managedCount);
            _children = new List<GameObject>(_managedCount);

            foreach (Transform child in transform.transform)
            {
                _children.Add(child.gameObject);
                _colliders.Add(child.GetComponent<Collider>());
                _renderers.Add(child.GetComponent<MeshRenderer>());
            }
        }

        public void Reset()
        {
            Transform.localScale = GameConstants.Animation.Obstacle.ResetScaleValue;
            foreach (var renderer in _renderers) renderer.enabled = true;
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

        public void Begin(float distanceToTravel, float time, Action onComplete)
        {
            Animate(time * _spawnData.AnimationTimeNormalization);
            StartMovement(distanceToTravel, time, onComplete);
            StartRotation(time * _spawnData.RotationTimeNormalization);
        }

        private void SetZ(float z)
        {
            MovementUtils.SetPositionForObstacle(Transform, z);
        }

        private void SetSpawnData()
        {
            for (var i = 0; i < _managedCount; i++) _renderers[i].enabled = _spawnData.IsEnabled(i);
            var spawnRotation = DeterministicRandomProvider.Next(_spawnData.SpawnRotation.Min, _spawnData.SpawnRotation.Max);
            MovementUtils.SetRotation(Transform, spawnRotation);
        }
        
        private void StartMovement(float distanceToTravel, float time, Action onComplete)
        {
            LeanTween.moveLocalZ(_go, -distanceToTravel, time).setOnComplete(onComplete);
        }
        
        private void StartRotation(float time)
        {
            LeanTween.rotateZ(_go,
                DeterministicRandomProvider.Next(_spawnData.TargetRotation.Min, _spawnData.TargetRotation.Max), time);
        }

        private void Animate(float time)
        {
            LeanTween.scale(_go, GameConstants.Animation.Obstacle.TargetScaleValue, time).setEase(GameConstants.Animation.Obstacle.Ease);
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
                public const LeanTweenType Ease = LeanTweenType.easeInOutSine;
            }
        }
    }
}