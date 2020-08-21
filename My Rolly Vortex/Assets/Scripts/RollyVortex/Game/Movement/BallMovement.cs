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
        }

        public void SetLevelData(float speed)
        {
            _speedMultiplier = _tiling / speed;
        }

        public void OnCollisionEnter(GameObject other)
        {
            if(other.tag.Equals(RollyVortexTags.Board)) IsEnabled = true;
        }

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