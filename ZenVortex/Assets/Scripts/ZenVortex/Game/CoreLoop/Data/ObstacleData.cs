using System.Collections.Generic;
using UnityEngine;

namespace ZenVortex
{
    public class ObstacleData : ScriptableObject
    {
        [Header("Gameplay")]
        [SerializeField] private int _points = 1;
        
        [Header("Spawn Data")] 
        [SerializeField] private IntRangedValue _spawnRotation;
        [SerializeField] private IntRangedValue _targetRotation;
        [SerializeField] private List<Color> _spawnColors = GameConstants.Animation.Obstacle.DefaultColors;

        [Header("Prefab Data")]
        [SerializeField] private List<int> _disabledObjects;
        [SerializeField] private List<int> _enabledObjects;
        
        [Header("Editor Utils")]
        [SerializeField] private bool _autoCalculateTarget;

        //Spawn
        public List<Color> SpawnColors => _spawnColors;
        public IntRangedValue SpawnRotation => _spawnRotation;
        public IntRangedValue TargetRotation => _targetRotation;
        public float AnimationInTimeNormalization => GameConstants.Animation.Obstacle.TravelTimeToAnimationRatio;
        
        //Points
        public int Points => _points; 
        
        public float RotationTimeNormalization(float visibility) => 1f / visibility - AnimationInTimeNormalization;
        
        public bool IsEnabled(int childIndex)
        {
            return _enabledObjects.Contains(childIndex);
        }
        
        public bool IsDisabled(int childIndex)
        {
            return _disabledObjects.Contains(childIndex);
        }

#if UNITY_EDITOR
        public void SerializeObstacleData(List<int> enabledObjects, List<int> disabledObjects)
        {
            _enabledObjects = new List<int>();
            foreach (var obj in enabledObjects) _enabledObjects.Add(obj);
            _disabledObjects = new List<int>();
            foreach (var obj in disabledObjects) _disabledObjects.Add(obj);
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void OnValidate()
        {
            if(!IsPrefabDataValid())
            {
                Debug.LogError($"[{nameof(ObstacleData)}] [{nameof(OnValidate)}] Invalid data. {name}");
                return;
            }
            
            if (!IntRangedValue.IsInitialized(_spawnRotation))
            {
                _spawnRotation = GameConstants.Animation.Obstacle.DefaultRotation;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
            if (!IntRangedValue.IsInitialized(_targetRotation))
            {
                _targetRotation = _autoCalculateTarget ? FindSafeTarget() : GameConstants.Animation.Obstacle.DefaultRotation;
                UnityEditor.EditorUtility.SetDirty(this);
            }

            if (_autoCalculateTarget && _targetRotation.IsEqualTo(GameConstants.Animation.Obstacle.DefaultRotation))
            {
                _targetRotation = FindSafeTarget();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        private IntRangedValue FindSafeTarget()
        {
            var totalObjects = _enabledObjects.Count + _disabledObjects.Count;
            
            var isPrevEnabled = _enabledObjects.Contains(0);
            var currentGapSize = isPrevEnabled ? 0 : 1;
            var gapStart = currentGapSize == 0 ? -1 : 0;
            var largestGapRange = new IntRangedValue(gapStart, gapStart, true);
            var smallestGapRange = new IntRangedValue(0, totalObjects - 1, true);
            
            for (var i = 1; i < totalObjects; i++)
            {
                var isCurrentEnabled = _enabledObjects.Contains(i);
                var didChange = isPrevEnabled != isCurrentEnabled;
                if (!didChange)
                {
                    if (!isPrevEnabled) continue;
                    gapStart = -1;
                }
                else
                {
                    if (isPrevEnabled)
                    {
                        gapStart = i;
                    }
                    else
                    {
                        var gap = new IntRangedValue(gapStart, i - 1, true);
                        
                        if(gap.Interval > largestGapRange.Interval && IsGapValid(gap))
                        {
                            largestGapRange = gap;
                        }

                        if (gap.Interval < smallestGapRange.Interval && IsGapValid(gap))
                        {
                            smallestGapRange = gap;
                        }
                    }
                }

                isPrevEnabled = isCurrentEnabled;
            }
            
            const float resolution = GameConstants.Animation.Obstacle.Resolution;
            const float buffer = GameConstants.Animation.Obstacle.MinGapSize * (360f / resolution);
            var safeRange = new IntRangedValue((int)(90 + buffer), (int) (270 - buffer));

            
            var largestGapAngle = new IntRangedValue((int)(largestGapRange.Min / resolution * 360f),
                (int)(largestGapRange.Max / resolution * 360f));
            
            var targetRotation = new IntRangedValue(safeRange.Min - largestGapAngle.Max, safeRange.Max - largestGapAngle.Min);

            return targetRotation;
        }

        private static bool IsGapValid(IntRangedValue gap)
        {
            return gap.Interval >= GameConstants.Animation.Obstacle.MinGapSize;
        }

        private bool IsPrefabDataValid()
        {
            return !(_enabledObjects == null || _disabledObjects == null) &&
                   _disabledObjects.Count + _enabledObjects.Count == GameConstants.Animation.Obstacle.Resolution &&
                   _disabledObjects.Count >= GameConstants.Animation.Obstacle.MinGapSize;
        }
#endif
    }

    public partial class GameConstants
    {
        internal static partial class Animation
        {
            internal static partial class Obstacle
            {
                public const float TravelTimeToAnimationRatio = 0.15f;
                public const int MinGapSize = 5;
                public const int Resolution = 24;
                public static readonly IntRangedValue DefaultRotation = new IntRangedValue(0, 360);
                public static readonly List<Color> DefaultColors = new List<Color>
                {
                    Color.black, Color.blue, Color.cyan, Color.green, Color.red, Color.yellow
                };
            }
        }
    }
}