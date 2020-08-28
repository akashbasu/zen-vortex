using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleData : ScriptableObject
    {
        [SerializeField] private List<int> _disabledObjects;
        [SerializeField] private List<int> _enabledObjects;

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
            EditorUtility.SetDirty(this);
        }
#endif
    }
}