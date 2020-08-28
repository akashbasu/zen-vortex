using UnityEditor;
using UnityEngine;

namespace RollyVortex
{
    public class LevelData : ScriptableObject
    {
        [SerializeField] private float _ballSpeed;

        [Header("Delay")] [SerializeField] private float _delayBeforeStart = 3f;

        [SerializeField] private RangedValue _grouping;
        [SerializeField] private float _obstacleSpeed;

        [Header("Difficulty")] [SerializeField]
        private float _speed = 1f;

        [Header("Speed")] [SerializeField] private float _tubeSpeed;

        [SerializeField] private int _visibility = 3;

        //Difficulty
        public int Visibility => _visibility;
        public RangedValue Grouping => _grouping;

        //Speed
        public float TubeSpeed => _tubeSpeed;
        public float ObstacleSpeed => _obstacleSpeed;
        public float BallSpeed => _ballSpeed;

        //Delay
        public float DelayBeforeStart => _delayBeforeStart;


#if UNITY_EDITOR
        private void OnValidate()
        {
            _tubeSpeed = _speed * GameConstants.EnvironmentConstants.MasterToTubeSpeedRatio;
            _obstacleSpeed = _speed * GameConstants.EnvironmentConstants.MasterToObstacleSpeedRatio;
            _ballSpeed = _speed * GameConstants.EnvironmentConstants.MasterToBallTilingRatio;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}