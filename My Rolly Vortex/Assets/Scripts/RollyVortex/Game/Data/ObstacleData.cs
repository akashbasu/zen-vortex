using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleData : ScriptableObject
    {
        private const float rotationTimeNormalized = 0.25f;
        private const float animationTimeNormalized = 0.5f;
        
        [Header("Spawn Data")] 
        [SerializeField] private IntRangedValue _spawnRotation;
        [SerializeField] private IntRangedValue _targetRotation;
        [SerializeField] private List<Color> _spawnColors = new List<Color>
            {Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.red, Color.magenta, Color.yellow};

        [Header("Prefab Data")]
        [SerializeField] private List<int> _disabledObjects;
        [SerializeField] private List<int> _enabledObjects;

        //Spawn
        public List<Color> SpawnColors => _spawnColors;
        public IntRangedValue SpawnRotation => _spawnRotation;
        public IntRangedValue TargetRotation => _targetRotation;
        public float AnimationTimeNormalized => animationTimeNormalized;
        public float RotationTimeNormalized => rotationTimeNormalized;

        public bool IsEnabled(int childIndex)
        {
            return _enabledObjects.Contains(childIndex);
        }

#if UNITY_EDITOR
        public void SerializeObstacleData(List<int> enabledObjects, List<int> disabledObjects)
        {
            _enabledObjects = new List<int>();
            foreach (var obj in enabledObjects) _enabledObjects.Add(obj);
            _disabledObjects = new List<int>();
            foreach (var obj in disabledObjects) _disabledObjects.Add(obj);
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void OnValidate()
        {
            var isChanged = !IntRangedValue.IsInitialized(_spawnRotation) || !IntRangedValue.IsInitialized(_targetRotation);
            if (!IntRangedValue.IsInitialized(_spawnRotation)) _spawnRotation = new IntRangedValue(0, 360);
            if (!IntRangedValue.IsInitialized(_targetRotation)) _targetRotation = new IntRangedValue(0, 360);
            
            if(isChanged) UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}