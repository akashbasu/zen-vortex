using System;
using UnityEngine;

namespace RollyVortex
{
    public sealed class BallMovement : ILevelMovement
    {
        private readonly GameObject _ball;
        
        private readonly Vector3 _cachedPosition;
        private readonly Rigidbody _rigidBody;
        
        private readonly Material _material;
        private readonly float _materialXOffset;
        private readonly int _textureId;
        private readonly float _tiling;
        private float _speedMultiplier;

        private float _currentOffset;
        private float _lastTime;

        private GameInputAdapter _input;
        
        public BallMovement(GameObject ball)
        {
            _ball = ball;
            
            _rigidBody = ball.GetComponent<Rigidbody>();
            _cachedPosition = ball.transform.position;
            
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

        public bool IsEnabled { get; set; } = false;

        public void Reset()
        {
            IsEnabled = false;
            
            _rigidBody.useGravity = false;
            _ball.transform.position = _cachedPosition;
            
            _lastTime = 0;
            _currentOffset = 0f;
            MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -_currentOffset);
        }

        public void Update(float deltaTime)
        {
            if(!IsEnabled) return;

            MovementUtils.UpdateTexturePositionY(ref _lastTime, ref _currentOffset, _tiling, deltaTime, _speedMultiplier, _material, _textureId);
            
            if (_input.TryGetInput(out float normalizedInput))
            {
                normalizedInput /= 2f;
                Debug.Log($"{nameof(BallMovement)} is trying to move ball by {normalizedInput}");
                // _rigidBody.AddForce(normalizedInput, 0f, 0f, ForceMode.VelocityChange);
            }
        }

        public void SetLevelData(LevelData data)
        {
            _speedMultiplier = _tiling / data.Speed;
        }

        public void OnCollisionStay(GameObject other)
        {
            if (!IsEnabled && other.tag.Equals(RollyVortexTags.Board))
            {
                IsEnabled = true;
                _input.SetGameInputEnabled(true);
                _rigidBody.position = new Vector3(0f, _rigidBody.position.y, _rigidBody.position.z);
            }
        }

        public void OnCollisionEnter(GameObject other)
        {}

        public void OnLevelEnd()
        {
            Reset();
        }

        public void OnLevelStart()
        {
            _rigidBody.useGravity = true;
        }

        public void OnCollisionExit(GameObject other) { }
    }
}