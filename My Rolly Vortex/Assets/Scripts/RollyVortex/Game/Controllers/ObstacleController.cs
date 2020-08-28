using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleController
    {
        private readonly List<GameObject> _children;
        private readonly List<Collider> _colliders;

        private readonly GameObject _go;
        private readonly int _managedCount;

        public readonly Transform Entry;

        public ObstacleController(Transform entry)
        {
            Entry = entry;
            _go = entry.gameObject;
            _managedCount = entry.childCount;

            _colliders = new List<Collider>(_managedCount);
            _children = new List<GameObject>(_managedCount);

            foreach (Transform child in entry.transform)
            {
                _children.Add(child.gameObject);
                _colliders.Add(child.GetComponent<Collider>());
            }
        }

        public void Reset()
        {
            Enable(false);
            SetZ(0f);
        }

        public void Spawn(ObstacleData data)
        {
            if (data == null)
            {
                Reset();
                return;
            }

            SetData(data);
            Enable(true);
        }

        public void Begin(float distanceToTravel, float time, Action onComplete)
        {
            Animate();
            StartMovement(distanceToTravel, time, onComplete);
            StartRotation();
        }

        private void StartMovement(float distanceToTravel, float time, Action onComplete)
        {
            LeanTween.moveLocalZ(_go, -distanceToTravel, time).setOnComplete(onComplete);
        }

        private void SetZ(float z)
        {
            MovementUtils.SetPositionForObstacle(Entry, z);
        }

        private void SetData(ObstacleData data)
        {
            for (var i = 0; i < _managedCount; i++) _children[i].SetActive(data.IsEnabled(i));
            //Set spawn rotation
        }

        private void Enable(bool isEnabled)
        {
            _go.SetActive(isEnabled);
            foreach (var collider in _colliders) collider.enabled = isEnabled;
        }

        private void Animate() { }

        private void StartRotation() { }
    }
}