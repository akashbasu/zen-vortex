using System;
using System.Collections.Generic;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class BallMovement : IGameLoopObserver, IPostConstructable
    {
        [Dependency] private readonly IInputServiceController _inputServiceController;
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly ISceneReferenceProvider _sceneReferenceProvider;
        
        private GameObject _ball;
        private Transform _anchor;

        private Material _material;
        private float _materialXOffset;
        private int _textureId;
        private float _tiling;

        private float _currentRotation;
        private float _loopInSeconds;
        private float _gravityTime;
        private float _xGravityClock;
        private float _xInputClock;
        private float _delayBeforeStart;

        private float _scaleFactor;
        private Vector3 _originalScale;
        private LTDescr _animationTween;
        private List<GameObject> _particleStates;

        private bool _isEnabled;

        public float CurrentScale
        {
            get => _scaleFactor;
            private set
            {
                _scaleFactor = value;
                new BallScaleUpdatedCommand().Execute();
            }
        }

        public void PostConstruct(params object[] args)
        {
            if (!_sceneReferenceProvider.TryGetEntry(Tags.Ball, out var ball))
            {
                Debug.LogError($"[{nameof(TubeMovement)}] Cannot find references");
                return;
            }
            
            _ball = ball;
            _originalScale = _ball.transform.localScale;
            CurrentScale = 1;
            _anchor = ball.transform.parent;

            var material = ball.GetComponent<Renderer>().material;

            if (material == null)
            {
                Debug.LogError($"[{nameof(BallMovement)}] cannot find material!");
                return;
            }

            _material = material;

            _textureId = material.GetTexturePropertyNameIDs()[0];
            _materialXOffset = material.GetTextureOffset(_textureId).x;
            _tiling = material.GetTextureScale(_textureId).y;

            _particleStates = new List<GameObject>();
            foreach (Transform child in ball.transform)
            {
                if (!string.Equals(child.name, GameConstants.Animation.Emitter)) continue;
                foreach (Transform particleObject in child.transform) _particleStates.Add(particleObject.gameObject);
            }
        }
        
        public void Dispose()
        {
            Reset();
        }

        public void Reset()
        {
            ParticleForState(AnimationState.Idle);
            StopMovement();
            ResetTween();
            ResetScale();
            
            _xInputClock = 0;
            _xGravityClock = 0f;
            _currentRotation = 0;
            _ball.transform.localScale = _originalScale;
            
            MovementUtils.SetBallRotation(_anchor, _currentRotation);
            MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, 0);
        }

        public void Update(float deltaTime)
        {
            if (!_isEnabled) return;
            
            if (_inputServiceController.GameInput.TryGetInput(out var normalizedInput))
            {
                _xGravityClock = 0f;
                
                var targetRotation = Mathf.Clamp(normalizedInput * 90f, -90f, 90f);
                if (Mathf.Approximately(targetRotation, _anchor.rotation.z))
                {
                    _xInputClock = 0f;
                    return;
                }

                MovementUtils.UpdateBallPosition(ref _xInputClock, _anchor, deltaTime, targetRotation, 1f);
            }
            else
            {
                _xInputClock = 0f;

                MovementUtils.UpdateBallPosition(ref _xGravityClock, _anchor, deltaTime, 0f, _gravityTime);
            }
        }

        public void SetLevelData(LevelData data)
        {
            _loopInSeconds = _tiling / data.BallSpeed;
            _gravityTime = 1f / data.Gravity;
            _delayBeforeStart = data.DelayBeforeStart;
        }

        public void OnGameEnd()
        {
            _gameEventManager.Unsubscribe(GameEvents.BallScale.Increment, StartGrowScaleTween);
            _gameEventManager.Unsubscribe(GameEvents.BallScale.Decrement, StartShrinkScaleTween);
            
            StopMovement();
            ParticleForState(AnimationState.Complete);
        }

        public void OnGameStart()
        {
            _gameEventManager.Subscribe(GameEvents.BallScale.Increment, StartGrowScaleTween);
            _gameEventManager.Subscribe(GameEvents.BallScale.Decrement, StartShrinkScaleTween);
            
            AnimateIntro();
            StartYAnimationTween();
            _isEnabled = true;
        }

        private void StopMovement()
        {
            _isEnabled = false;
            _animationTween?.pause();
        }

        private void AnimateIntro()
        {
            ParticleForState(AnimationState.Moving);
            LeanTween.value(0f, _tiling, _delayBeforeStart / GameConstants.Animation.Ball.LoopsBeforeStart).setLoopClamp().setOnUpdate(
                tiling =>
                    MovementUtils.SetTexturePosition(_material, _textureId, -tiling, 0)).setRepeat((int) GameConstants.Animation.Ball.LoopsBeforeStart);
        }
        
        private void StartYAnimationTween()
        {
            _animationTween = LeanTween.value(0f, _tiling, _loopInSeconds).setLoopClamp().setOnUpdate(tiling =>
                MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -tiling)).setDelay(_delayBeforeStart);
        }

        private void ResetScale()
        {
            CurrentScale = 1;
            _ball.transform.localScale = _originalScale;
        }
        
        private void StartGrowScaleTween(object[] args)
        {
            if(args?.Length < 1 || !(args[0] is float increment)) return;

            CurrentScale = Math.Min(CurrentScale + increment, GameConstants.Animation.Ball.MaxScale);
            var targetScale = _originalScale * CurrentScale;
            
            ScaleToTarget(targetScale);
            
            Debug.Log($"[{nameof(BallMovement)}] {nameof(StartGrowScaleTween)} Scale factor {CurrentScale}");
        }
        
        private void StartShrinkScaleTween(object[] args)
        {
            if(args?.Length < 1 || !(args[0] is float decrement)) return;

            CurrentScale = Math.Max(CurrentScale - decrement, GameConstants.Animation.Ball.MinScale);
            var targetScale = _originalScale * CurrentScale;
            
            ScaleToTarget(targetScale);
            
            Debug.Log($"[{nameof(BallMovement)}] {nameof(StartShrinkScaleTween)} Scale factor {CurrentScale}");
        }

        private void ScaleToTarget(Vector3 targetScale)
        {
            if (_ball.LeanIsTweening())
            {
                LeanTween.cancel(_ball);
            }
            
            _ball.LeanScale(targetScale, GameConstants.Animation.Ball.TimeToChangeScale).setEase(GameConstants.Animation.Ball.Ease);
        }

        private void ResetTween()
        {
            if (_animationTween != null)
            {
                _animationTween.pause();

                LeanTween.cancel(_animationTween.uniqueId);
                _animationTween.reset();
                _animationTween = null;
            }

            if (_ball.LeanIsTweening())
            {
                LeanTween.cancel(_ball);
            }
        }

        private void ParticleForState(AnimationState state)
        {
            var index = (int) state;
            foreach (var particleState in _particleStates) particleState.SetActive(false);
            
            if(_particleStates.Count > 0 && index < _particleStates.Count)
            {
                _particleStates[index].SetActive(true);
                var ps = _particleStates[index].GetComponent<ParticleSystem>();
                if(ps != null) ps.Play();
            }
        }
    }
    
    
    public static partial class GameConstants
    {
        internal static partial class Animation
        {
            internal static partial class Ball
            {
                public const float LoopsBeforeStart = 5f;
                public const float TimeToChangeScale = 0.5f;
                
                public const float MaxScale = 1f;
                public const float MinScale = 0.35f;
                public const LeanTweenType Ease = LeanTweenType.easeInOutSine;
            }

            public const string Emitter = nameof(Emitter);
        }
    }
    
    internal partial class UiDataKeys
    {
        internal partial class Ball
        {
            public static readonly string Scale = $"{nameof(Ball)}.{nameof(Scale)}";
        }
    }
}