using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class TubeMovement : IGameLoopObserver, IPostConstructable
    {
        [Dependency] private readonly ISceneReferenceProvider _sceneReferenceProvider;
        
        private Material _material;
        private float _materialXOffset;
        private int _textureId;
        private float _tiling;
        private float _loopInSeconds;
        private float _delayBeforeStart;

        private LTDescr _animationTween;
        
        public void PostConstruct(params object[] args)
        {
            if (!_sceneReferenceProvider.TryGetEntry(Tags.Board, out var tube))
            {
                Debug.LogError($"[{nameof(TubeMovement)}] Cannot find references");
                return;
            }
            
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
        
        public void Dispose()
        {
            Reset();
        }

        public void Reset()
        {
            ResetTween();
            MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, 0f);
        }

        public void SetLevelData(LevelData data)
        {
            _loopInSeconds = _tiling / data.TubeSpeed;
            _delayBeforeStart = data.DelayBeforeStart;
        }

        public void OnGameEnd()
        {
            StopMovement();
        }

        private void StopMovement()
        {
            _animationTween?.pause();
        }

        public void OnGameStart()
        {
            StartTween();
        }

        private void StartTween()
        {
            _animationTween = LeanTween.value(0f, _tiling, _loopInSeconds).setLoopClamp().setOnUpdate(tiling =>
                MovementUtils.SetTexturePosition(_material, _textureId, _materialXOffset, -tiling)).setDelay(_delayBeforeStart);
        }

        private void ResetTween()
        {
            if (_animationTween == null) return;

            StopMovement();
            
            LeanTween.cancel(_animationTween.uniqueId);
            _animationTween.reset();
            _animationTween = null;
        }
        
        public void Update(float deltaTime) { }
    }
}