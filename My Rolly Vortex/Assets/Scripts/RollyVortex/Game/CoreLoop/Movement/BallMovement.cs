using UnityEngine;

namespace RollyVortex
{
    internal sealed class BallMovement : ILevelMovement
    {
        private readonly Transform _anchor;

        // private readonly GameInputAdapter _input;

        private readonly Material _material;
        private readonly float _materialXOffset;
        private readonly int _textureId;
        private readonly float _tiling;

        private float _currentRotation;
        private float _loopInSeconds;
        private float _gravityTime;
        private float _xGravityClock;
        private float _xInputClock;

        private LTDescr _animationTween;

        private bool _isEnabled;
        
        internal BallMovement(GameObject ball)
        {
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

            // _input = new GameInputAdapter();
        }

        public void Reset()
        {
            _isEnabled = false;
            
            _xInputClock = 0;
            _xGravityClock = 0f;
            _currentRotation = 0;
            
            MovementUtils.SetBallRotation(_anchor, _currentRotation);
            
            StopTween();
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
            Reset();
        }

        public void OnLevelStart()
        {
            AnimateIntro();
            StartYAnimationTween();
            _isEnabled = true;
        }

        private void AnimateIntro()
        {
            LeanTween.value(0f, _tiling, LevelDataProvider.LevelData.DelayBeforeStart / GameConstants.Animation.Ball.LoopsBeforeStart).setLoopClamp().setOnUpdate(
                tiling =>
                    MovementUtils.SetTexturePosition(_material, _textureId, -tiling, 0)).setRepeat((int) GameConstants.Animation.Ball.LoopsBeforeStart);
        }
        
        private void StartYAnimationTween()
        {
            _animationTween = LeanTween.value(0f, _tiling, _loopInSeconds).setLoopClamp().setOnUpdate(tiling =>
                MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -tiling)).setDelay(LevelDataProvider.LevelData.DelayBeforeStart);
        }

        private void StopTween()
        {
            if (_animationTween == null) return;
            
            LeanTween.cancel(_animationTween.uniqueId);
            _animationTween.reset();
            _animationTween = null;
        }

        private bool IsCollisionFatal(GameObject other)
        {
            return false;
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
        }
    }
}