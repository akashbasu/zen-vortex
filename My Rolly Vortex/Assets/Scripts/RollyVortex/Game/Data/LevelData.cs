using System;
using UnityEditor;
using UnityEngine;

namespace RollyVortex
{
    public class LevelData : ScriptableObject
    {
        [Header("Difficulty")] 
        [SerializeField] private float _speed = 1f;
        [SerializeField] private int _visibility = 3;
        [SerializeField] private IntRangedValue _grouping;

        [Header("Environment")] 
        [SerializeField] private float _gravity = 1f;
        
        [Header("Speed")]
        [SerializeField] private float _tubeSpeed;
        [SerializeField] private float _ballSpeed;
        [SerializeField] private float _obstacleSpeed;

        [Header("Delay")] 
        [SerializeField] private float _delayBeforeStart = 3f;

        //Difficulty
        public int Visibility => _visibility;
        public IntRangedValue Grouping => _grouping;

        //Speed
        public float TubeSpeed => _tubeSpeed;
        public float ObstacleSpeed => _obstacleSpeed;
        public float BallSpeed => _ballSpeed;
        
        //Environment
        public float Gravity => _gravity;

        //Delay
        public float DelayBeforeStart => _delayBeforeStart;
        public int Seed => (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;


#if UNITY_EDITOR
        private void OnValidate()
        {
            _tubeSpeed = _speed * GameConstants.Environment.MasterToTubeSpeedRatio;
            _obstacleSpeed = _speed * GameConstants.Environment.MasterToObstacleSpeedRatio;
            _ballSpeed = _speed * GameConstants.Environment.MasterToBallTilingRatio;
            EditorUtility.SetDirty(this);
        }
#endif
    }

    public partial class GameConstants
    {
        public static class Environment
        {
            public const float MasterToTubeSpeedRatio = 1f;
            public const float MasterToObstacleSpeedRatio = 2f;
            public const float MasterToBallTilingRatio = 1f;
        }
    }
}