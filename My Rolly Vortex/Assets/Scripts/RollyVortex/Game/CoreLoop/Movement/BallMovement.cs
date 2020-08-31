using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    internal sealed class BallMovement : ILevelMovement
    {
        private readonly GameObject _ball;
        private readonly Transform _anchor;

        private readonly Material _material;
        private readonly float _materialXOffset;
        private readonly int _textureId;
        private readonly float _tiling;

        private float _currentRotation;
        private float _loopInSeconds;
        private float _gravityTime;
        private float _xGravityClock;
        private float _xInputClock;

        private Vector3 _originalScale;
        private LTSeq _scaleSequence;
        private LTDescr _animationTween;
        private List<GameObject> _particleStates;

        private bool _isEnabled;
        
        internal BallMovement(GameObject ball)
        {
            _ball = ball;
            _originalScale = _ball.transform.localScale;
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

        public void Reset()
        {
            ParticleForState(AnimationState.Idle);
            StopMovement();
            ResetTween();
            
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
            
            if (InputController.GameInput.TryGetInput(out var normalizedInput))
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
        }

        public void OnCollisionEnter(GameObject other, int pointOfCollision) { }

        public void OnCollisionExit(GameObject other, int pointOfCollision) { }

        public void OnLevelEnd()
        {
            GameEventManager.Unsubscribe(GameEvents.Gameplay.OverrideSize, StartScaleTween);
            
            StopMovement();
            ParticleForState(AnimationState.Complete);
        }

        public void OnLevelStart()
        {
            GameEventManager.Subscribe(GameEvents.Gameplay.OverrideSize, StartScaleTween);
            
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
            LeanTween.value(0f, _tiling, LevelDataProvider.LevelData.DelayBeforeStart / GameConstants.Animation.Ball.LoopsBeforeStart).setLoopClamp().setOnUpdate(
                tiling =>
                    MovementUtils.SetTexturePosition(_material, _textureId, -tiling, 0)).setRepeat((int) GameConstants.Animation.Ball.LoopsBeforeStart);
        }
        
        private void StartYAnimationTween()
        {
            _animationTween = LeanTween.value(0f, _tiling, _loopInSeconds).setLoopClamp().setOnUpdate(tiling =>
                MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -tiling)).setDelay(LevelDataProvider.LevelData.DelayBeforeStart);
        }

        private void StartScaleTween(object[] args)
        {
            if(args?.Length < 2) return;
            float scaleFactor = (float) args[0], duration = (float) args[1];
            
            if (_scaleSequence != null && LeanTween.isTweening(_scaleSequence.id))
            {
                var currentScale = _ball.transform.localScale;
                LeanTween.cancel(_scaleSequence.id);
                _ball.transform.localScale = currentScale;
            }
            
            var targetScale = _originalScale * scaleFactor;

            _scaleSequence = LeanTween.sequence().append(LeanTween.scale(_ball, targetScale, duration * .1f)).append(duration)
                .append(LeanTween.scale(_ball, _originalScale, duration * .1f));
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

            if (_scaleSequence != null)
            {
                LeanTween.cancel(_scaleSequence.id);
                _scaleSequence.reset();
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
        
        public void OnCollisionStay(GameObject other) { }
    }
    
    
    public static partial class GameConstants
    {
        internal static partial class Animation
        {
            internal static partial class Ball
            {
                public const float LoopsBeforeStart = 5f;
            }

            public const string Emitter = nameof(Emitter);
        }
    }
}