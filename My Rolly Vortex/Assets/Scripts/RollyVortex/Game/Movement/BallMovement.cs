using UnityEngine;

namespace RollyVortex
{
    public sealed class BallMovement : ILevelMovement
    {
        private readonly Transform _anchor;
        private readonly GameObject _ball;

        private readonly GameInputAdapter _input;

        private readonly Material _material;
        private readonly float _materialXOffset;
        private readonly int _textureId;
        private readonly float _tiling;
        private float _currentOffset;

        private float _currentRotation;
        private float _speedMultiplier;
        private float _xGravityClock;
        private float _xInputClock;

        private float _yClock;

        public BallMovement(GameObject ball)
        {
            _ball = ball;

            _anchor = _ball.transform.parent;

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

            _input = new GameInputAdapter();
        }

        public bool IsEnabled { get; set; }

        public void Reset()
        {
            IsEnabled = false;

            _yClock = 0;
            _xInputClock = 0;
            _xGravityClock = 0f;

            _currentOffset = 0f;
            _currentRotation = 0;
            MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -_currentOffset);
            MovementUtils.SetBallRotation(_anchor, _currentRotation);
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled) return;

            MovementUtils.UpdateTexturePositionY(ref _yClock, ref _currentOffset, _tiling, deltaTime, _speedMultiplier,
                _material, _textureId);

            if (_input.TryGetInput(out var normalizedInput))
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
                MovementUtils.UpdateBallPosition(ref _xGravityClock, _anchor, deltaTime, 0f, 1f);
            }
        }

        public void SetLevelData(LevelData data)
        {
            _speedMultiplier = _tiling / data.Speed;
        }

        public void OnCollisionStay(GameObject other)
        {
        }

        public void OnCollisionEnter(GameObject other)
        {
        }

        public void OnCollisionExit(GameObject other)
        {
        }

        public void OnLevelEnd()
        {
            Reset();
        }

        public void OnLevelStart()
        {
            if (IsEnabled) return;

            _input.SetGameInputEnabled(true);
            IsEnabled = true;
        }
    }
}