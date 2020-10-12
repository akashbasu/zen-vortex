using System;
using UnityEngine;

namespace ZenVortex
{
    [Serializable]
    public enum PowerupType
    {
        None = 0,
        Lives = 1,
        Shrink = 2,
        Time = 3
    }

    [Serializable]
    public enum PowerupEffect
    {
        None = 0,
        Counted = 1,
        Timed = 2
    }
    
    public class PowerupData : ScriptableObject
    {
        [SerializeField] private PowerupType _type;
        [SerializeField] private PowerupEffect _effect;
        [SerializeField] private float _data;
        
        [Header("Gameplay")]
        [SerializeField] private int _points = 1;
        
        [Header("Visuals")]
        [SerializeField] private Texture _image;
        
        [Header("Animation Constants")]
        [SerializeField] private IntRangedValue _spawnRotation = GameConstants.Animation.Powerup.SafeRotationRange;
        [SerializeField] private float _rotationTimeNormalization = 0.2f;
        
        //Type
        public PowerupType Type => _type;
        public PowerupEffect Effect => _effect;
        public float Data => _data;
        public int Points => _points;
        
        //Visuals
        public Texture Image => _image;
        
        //Animation
        public IntRangedValue SpawnRotation => _spawnRotation;
        public float RotationTimeNormalization => _rotationTimeNormalization;
    }
    
    public static partial class GameConstants
    {
        internal static partial class Animation
        {
            internal static partial class Powerup
            {
                private const float SafeRotation = 90 - (Obstacle.MinGapSize / 2f) * (360f / Obstacle.Resolution);
                public static readonly IntRangedValue SafeRotationRange = new IntRangedValue((int)-SafeRotation, (int)SafeRotation);
            }
        }
    }
}