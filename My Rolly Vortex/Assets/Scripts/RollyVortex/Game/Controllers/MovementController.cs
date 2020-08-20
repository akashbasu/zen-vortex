using System;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    public class MovementController : MonoBehaviour, IInitializable
    {
        private TubeMovement _tubeMovement;
        
        private bool _canMove;
        
        private Vector3 _cachedBallPosition;
        private GameObject _tube;
        private GameObject _ball;
        private Rigidbody _ballRB;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                ResetLevelValues();
                
                GameEventManager.Subscribe(GameEvents.LevelEvents.StartLevel, OnLevelStart);
                GameEventManager.Subscribe(GameEvents.Gameplay.CollisionStart, OnCollision);
                GameEventManager.Subscribe(GameEvents.LevelEvents.StopLevel, OnLevelStop);
                
                onComplete?.Invoke(this);
                return;
            }
            
            Debug.LogError($"[{nameof(MovementController)}] Cannot find references");
        }

        private bool GetReferences()
        {
            if (!DirectoryManager.TryGetEntry(RollyVortexTags.Board, out _tube)) return false;

            var material = _tube.GetComponent<Renderer>().material;

            if (material == null)
            {
                Debug.LogError($"[{nameof(MovementController)}] cannot find tube material!");
                return false;
            }

            _tubeMovement = new TubeMovement(material);
            
            if (!DirectoryManager.TryGetEntry(RollyVortexTags.Ball, out _ball)) return false;

            _ballRB = _ball.GetComponent<Rigidbody>();
            _cachedBallPosition = _ball.transform.position;
            
            
            return _ballRB != null && _cachedBallPosition != null;
        }
        
        private void ResetLevelValues()
        {
            _ballRB.useGravity = false;
            _ball.transform.position = _cachedBallPosition;

            _tubeMovement.Reset();
        }
        
        private void OnLevelStart(object[] args)
        {
            _ballRB.useGravity = true;

            if (args?.Length > 0)
            {
                var speed = (float) args[0];
                _tubeMovement.SetSpeed(speed);
            }
        }
        
        private void OnCollision(object[] obj)
        {
            if (obj?.Length >= 2)
            {
                var source = obj[0] as GameObject;
                var collidedWith = obj[1] as GameObject;

                if (source == _ball && collidedWith == _tube)
                {
                    _canMove = true;
                }
            }
        }
        
        private void OnLevelStop(object[] args)
        {
            ResetLevelValues();
        }

        private void Update()
        {
            if(!_canMove) return;

            _tubeMovement.Update(Time.deltaTime);
        }
    }

    public sealed class TubeMovement
    {
        private bool _canMove;
        private float _currentOffset;
        private float _lastTime;
        
        private readonly float _materialXOffset;
        private readonly Material _material;
        private readonly int _textureId;
        private readonly float _tiling;
        private float _speedMultiplier;
        
        public TubeMovement(Material material, float speed = 1f)
        {
            _material = material;
            _textureId = material.GetTexturePropertyNameIDs()[0];
            _materialXOffset = material.GetTextureOffset(_textureId).x;
            _tiling = material.GetTextureScale(_textureId).y;
            SetSpeed(speed);
        }
        
        public void SetSpeed(float speed)
        {
            _speedMultiplier = _tiling / speed;
        }
        
        public void Reset()
        {
            _lastTime = 0;
            _currentOffset = 0f;
            SetPosition();
        }
        
        public float Update(float deltaTime)
        {
            _lastTime += deltaTime;
            _lastTime %= _speedMultiplier;
            _currentOffset = Mathf.Lerp(0, _tiling, _lastTime / _speedMultiplier);
            _currentOffset %= _tiling;
            SetPosition();

            return _currentOffset;
        }
        
        private void SetPosition()
        {
            _material.SetTextureOffset(_textureId, new Vector2(_materialXOffset, -_currentOffset));
        }
    }
    
}