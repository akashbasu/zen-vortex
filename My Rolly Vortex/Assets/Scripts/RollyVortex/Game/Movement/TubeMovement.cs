using UnityEngine;

namespace RollyVortex
{
    public sealed class TubeMovement : ILevelMovement
    {
        private readonly Material _material;
        private readonly float _materialXOffset;
        private readonly int _textureId;
        private readonly float _tiling;
        private float _currentOffset;
        private float _lastTime;
        private float _speedMultiplier;

        public TubeMovement(GameObject tube)
        {
            var material = tube.GetComponent<Renderer>().material;

            if (material == null)
            {
                Debug.LogError($"[{nameof(TubeMovement)}] cannot find material!");
                return;
            }

            _material = material;

            _textureId = material.GetTexturePropertyNameIDs()[0];
            _materialXOffset = material.GetTextureOffset(_textureId).x;
            _tiling = material.GetTextureScale(_textureId).y;
        }

        public bool IsEnabled { get; set; }

        public void Reset()
        {
            IsEnabled = false;
            _lastTime = 0;
            _currentOffset = 0f;
            MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -_currentOffset);
        }

        public void Update(float deltaTime)
        {
            if (!IsEnabled) return;

            MovementUtils.UpdateTexturePositionY(ref _lastTime, ref _currentOffset, _tiling, deltaTime,
                _speedMultiplier, _material, _textureId);
        }

        public void SetLevelData(LevelData data)
        {
            _speedMultiplier = _tiling / data.Speed;
        }

        public void OnCollisionStay(GameObject other)
        {
        }

        public void OnLevelEnd()
        {
            Reset();
        }

        public void OnLevelStart()
        {
            IsEnabled = true;
        }

        public void OnCollisionEnter(GameObject other)
        {
        }

        public void OnCollisionExit(GameObject other)
        {
        }
    }
}