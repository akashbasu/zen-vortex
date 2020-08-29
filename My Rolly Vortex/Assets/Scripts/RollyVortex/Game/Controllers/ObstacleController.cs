using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleController
    {
        private readonly Vector3 _animateInResetScaleValue = new Vector3(2f, 2f, 2f);
        private readonly Vector3 _animateInTargetScaleValue = Vector3.one;
        
        private readonly List<GameObject> _children;
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
            _children = new List<GameObject>(_managedCount);

            foreach (Transform child in transform.transform)
            {
                _children.Add(child.gameObject);
                _colliders.Add(child.GetComponent<Collider>());
            }
        }

        public void Reset()
        {
            Transform.localScale = _animateInResetScaleValue;
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

        public void Begin(float distanceToTravel, float time, Action onComplete)
        {
            Animate(time * _spawnData.AnimationTimeNormalized);
            StartMovement(distanceToTravel, time, onComplete);
            StartRotation(time * _spawnData.RotationTimeNormalized);
        }

        private void SetZ(float z)
        {
            MovementUtils.SetPositionForObstacle(Transform, z);
        }

        private void SetSpawnData()
        {
            for (var i = 0; i < _managedCount; i++) _children[i].SetActive(_spawnData.IsEnabled(i));
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
            LeanTween.scale(_go, _animateInTargetScaleValue, time);
        }

        private void Enable(bool isEnabled)
        {
            _go.SetActive(isEnabled);
            foreach (var collider in _colliders) collider.enabled = isEnabled;
        }
    }
}