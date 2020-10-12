using System;
using System.Collections.Generic;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class PowerupController : ICacheEntry
    {
        [Dependency] private readonly IDeterministicRandomProvider _deterministicRandomProvider;
        
        private readonly List<MeshRenderer> _renderers;
        private readonly List<Collider> _colliders;
        
        private readonly GameObject _go;
        private readonly int _managedCount;
        
        private PowerupData _powerupData;

        public PowerupController(Transform transform)
        {
            Injector.ResolveDependencies(this);
            
            Transform = transform;
            _go = transform.gameObject;
            _managedCount = transform.childCount;
            
            _colliders = new List<Collider>(_managedCount);
            _renderers = new  List<MeshRenderer>(_managedCount);
            
            foreach (Transform child in transform.transform)
            {
                _colliders.Add(child.GetComponent<Collider>());
                var renderer = child.GetComponent<MeshRenderer>();
                _renderers.Add(renderer);
            }
        }
        
        public Transform Transform { get; }
        public bool HasActionableCollision => true;
        
        public void Reset()
        {
            LeanTween.cancel(_go);
            
            Enable(false);
            SetZ(0f);
        }

        public void Spawn(params object[] args)
        {
            if (args?.Length == 0 || !(args[0] is PowerupData spawnData))
            {
                Reset();
                return;
            }
            
            _powerupData = spawnData;

            SetSpawnData();
            Enable(true);
        }

        public void Fire(float distanceToTravel, float time, Action onComplete, params object[] args)
        {
            Animate(time * _powerupData.RotationTimeNormalization);
            StartMovement(distanceToTravel, time, onComplete);
        }

        public void Pause()
        {
            LeanTween.pause(_go);
        }
        
        public void CollisionStart(params object[] args)
        {
            Pickup();
        }

        public void CollisionComplete(params object[] args)
        {
            Enable(false);
        }
        
        private void SetZ(float z)
        {
            MovementUtils.SetPositionForObstacle(Transform, z);
        }
        
        private void Enable(bool isEnabled)
        {
            _go.SetActive(isEnabled);
            foreach (var collider in _colliders) collider.enabled = isEnabled;
        }
        
        private void SetSpawnData()
        {
            for (var i = 0; i < _managedCount; i++) _renderers[i].material.mainTexture = _powerupData.Image;
            
            var spawnRotation = _deterministicRandomProvider.Next(_powerupData.SpawnRotation);
            MovementUtils.SetRotation(Transform, spawnRotation);
        }

        private void Animate(float time)
        {
            LeanTween.rotateAroundLocal(_go, Vector3.up, 360f, time).setRepeat(-1);
        }
        
        private void StartMovement(float distanceToTravel, float time, Action onComplete)
        {
            Transform.LeanMoveLocalZ(-distanceToTravel, time).setOnComplete(onComplete);
        }

        private void Pickup()
        {
            new EventCommand(GameEvents.Powerup.Collect, _powerupData).Execute();
        }
    }
}