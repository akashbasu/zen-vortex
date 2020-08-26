using UnityEngine;

namespace RollyVortex
{
    public class LevelData : ScriptableObject
    {
        [SerializeField] private float _tubeSpeed;
        [SerializeField] private float _delayBeforeStart;
        [SerializeField] private float _obstacleGroupingDelay;

        public float TubeSpeed => _tubeSpeed;
        public float DelayBeforeStart => _delayBeforeStart;
        public float ObstacleSpeed => _tubeSpeed * GameConstants.TubeToTilingRatio;
        public float BallSpeed => _tubeSpeed * GameConstants.TubeToBallTilingRatio;

        public float ObstacleGroupingDelay => _obstacleGroupingDelay;

        public void SetDefault()
        {
            _tubeSpeed = 4f;
            _delayBeforeStart = 4f;
            _obstacleGroupingDelay = 1f;
        }
    }
}