using UnityEngine;

namespace RollyVortex
{
    public sealed class TubeMovement : ILevelMovement
    {
        private readonly Material _material;
        private readonly float _materialXOffset;
        private readonly int _textureId;
        private readonly float _tiling;
        private float _loopInSeconds;

        private LTDescr _animationTween;

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

        public void Reset()
        {
            StopTween();
            MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, 0f);
        }

        public void SetLevelData(LevelData data)
        {
            _loopInSeconds = _tiling / data.TubeSpeed;
        }

        public void OnLevelEnd()
        {
            Reset();
        }

        public void OnLevelStart()
        {
            StartTween();
        }

        private void StartTween()
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
        
        public void Update(float deltaTime) { }
        public void OnCollisionStay(GameObject other) { }
        public void OnCollisionEnter(GameObject other, int pointOfCollision) { }
        public void OnCollisionExit(GameObject other, int pointOfCollision) { }
    }
}