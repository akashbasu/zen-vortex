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

    internal interface IPowerupSpawnData
    {
        Texture Image { get; }
        IntRangedValue SpawnRotation { get; }
        float RotationTimeNormalization { get; }
    }

    internal interface IBasePowerupData : IPowerupSpawnData
    {
        PowerupType Type  { get; }
        int Points { get; }
    }
    
    public class BasePowerup : ScriptableObject, IBasePowerupData
    {
        [SerializeField] private PowerupType _type;
        [SerializeField] private int _points = 3;
        
        [Header("Visuals")]
        [SerializeField] private Texture _image;
        
        [Header("Animation Constants")]
        [SerializeField] private IntRangedValue _spawnRotation = GameConstants.Animation.Powerup.SafeRotationRange;
        [SerializeField] private float _rotationTimeNormalization = 0.2f;

        PowerupType IBasePowerupData.Type => _type;
        int IBasePowerupData.Points => _points;
        
        Texture IPowerupSpawnData.Image => _image;
        IntRangedValue IPowerupSpawnData.SpawnRotation => _spawnRotation;
        float IPowerupSpawnData.RotationTimeNormalization => _rotationTimeNormalization;
        
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