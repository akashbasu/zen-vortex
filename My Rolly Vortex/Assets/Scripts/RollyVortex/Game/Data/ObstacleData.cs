using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleData : ScriptableObject
    {
        [SerializeField] private List<int> _disabledObjects;
        [SerializeField] private List<int> _enabledObjects;
        [SerializeField] private IntRangedValue _spawnRotation;
        [SerializeField] private IntRangedValue _targetRotation;

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