using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZenVortex
{
    internal class ObstacleController : ICacheEntry
    {
        private readonly List<MeshRenderer> _renderers;
        private readonly List<Collider> _colliders;
        private readonly HashSet<int> _fatalCollisions;
        private readonly HashSet<int> _currentCollisions;

        private readonly GameObject _go;
        private readonly int _managedCount;
        
        private ObstacleData _spawnData;
        private bool _didConsumeLife;

        public Transform Transform { get; }
        public bool HasActionableCollision => _currentCollisions.Any(x => _fatalCollisions.Contains(x)) && PlayerDataProvider.LifeCount <= 0;

        //Runtime injection
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

            _didConsumeLife = false;
            Transform.localScale = GameConstants.Animation.Obstacle.ResetScaleValue;
            foreach (var renderer in _renderers) renderer.enabled = true;
            _fatalCollisions.Clear();
            _currentCollisions.Clear();
            AnimateColor(Color.clear, 0, 0);
            Enable(false);
            SetZ(0f);
        }

        public void Spawn(params object[] args)
        {
            if (args?.Length == 0 || !(args[0] is ObstacleData spawnData))
            {
                Reset();
                return;
            }

            _spawnData = spawnData;

            SetSpawnData();
            Enable(true);
        }

        public void Fire(float distanceToTravel, float time, Action onComplete, params object[] args)
        {
            if (!(args?.Length == 0 || !(args[0] is Color groupColor)))
            {
                AnimateIn(distanceToTravel, groupColor, time, onComplete);
            }
            else
            {
                Debug.LogError($"[{nameof(ObstacleController)}] {nameof(Fire)} did not find argument with obstacle group color.");
                AnimateIn(distanceToTravel, Color.magenta, time, onComplete);
            }
        }

        public void Pause()
        {
            LeanTween.pause(_go);
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
                new Command(GameEvents.Gameplay.ConsumeLife).Execute();
            }
            
            new Command(GameEvents.Gameplay.CrossedObstacle).Execute();
            AnimateOut(GameConstants.Animation.Obstacle.ResetScaleValue, GameConstants.Animation.Obstacle.AnimateOutTime);
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
            
            var spawnRotation = DeterministicRandomProvider.Next(_spawnData.SpawnRotation);
            MovementUtils.SetRotation(Transform, spawnRotation);
        }
        
        private void AnimateIn(float distanceToTravel, Color groupColor, float time, Action onComplete)
        {
            var animateInTime = time * _spawnData.AnimationInTimeNormalization;

            LeanTween.sequence()
                .append(() => StartMovement(distanceToTravel, time, onComplete))
                .append(() => AnimateScale(GameConstants.Animation.Obstacle.TargetScaleValue, animateInTime))
                .append(() => AnimateColor(groupColor, 1, animateInTime))
                .append(animateInTime)
                .append(() => StartRotation(time * _spawnData.RotationTimeNormalization));
        }
        
        private void AnimateOut(Vector3 sizeTarget, float time)
        {
            AnimateScale(sizeTarget, time);
        }
        
        private void StartMovement(float distanceToTravel, float time, Action onComplete)
        {
            _go.LeanMoveLocalZ(-distanceToTravel, time).setOnComplete(onComplete);
        }
        
        private void AnimateScale(Vector3 targetScale, float time)
        {
            _go.LeanScale(targetScale, time).setEase(GameConstants.Animation.Obstacle.Ease);
        }
        
        private void AnimateColor(Color targetColor, float alpha, float time)
        {
            _go.LeanColor(targetColor, 0);
            _go.LeanAlpha(alpha, time);
        }
        
        private void StartRotation(float time)
        {
            if (DeterministicRandomProvider.NextBool(_spawnData.RotationProbability))
            {
                _go.LeanRotateZ(DeterministicRandomProvider.Next(_spawnData.TargetRotation), time)
                    .setEase(GameConstants.Animation.Obstacle.Ease);    
            }
        }

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