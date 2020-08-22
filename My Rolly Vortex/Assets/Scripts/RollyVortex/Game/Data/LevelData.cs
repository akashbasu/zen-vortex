using UnityEngine;

namespace RollyVortex
{
    public class LevelData : ScriptableObject
    {
        [SerializeField] private float _speed;

        public float Speed => _speed;
        
        public void SetDefault()
        {
            _speed = 5f;
        }
    }
}