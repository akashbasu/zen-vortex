using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RollyVortex.Editor
{
    public class DataTools
    {
        [MenuItem("RollyVortex/DataTools/Level Data")]
        public static void CreateLevelData()
        {
            var newLevelData = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(newLevelData,
                Path.Combine("Assets", "Resources", GameConstants.DataPaths.LevelDataPath, "Level_.asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("RollyVortex/DataTools/Obstacle Data")]
        public static void CreateObstacleData()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null) return;

            var newObstacleData = ScriptableObject.CreateInstance<ObstacleData>();
            List<int> activated = new List<int>(), deactivated = new List<int>();
            foreach (Transform child in selectedObject.transform)
                if (child.gameObject.activeInHierarchy) activated.Add(child.GetSiblingIndex());
                else deactivated.Add(child.GetSiblingIndex());
            newObstacleData.SerializeObstacleData(activated, deactivated);
            AssetDatabase.CreateAsset(newObstacleData,
                Path.Combine("Assets", "Resources", GameConstants.DataPaths.ObstacleDataPath,
                    $"Obstacle_{selectedObject.name}.asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("RollyVortex/DataTools/Obstacle Data", true)]
        public static bool CanCreateObstacleData()
        {
            var activeGo = Selection.activeGameObject;
            return activeGo != null && string.Equals(activeGo.tag, RollyVortexTags.Obstacle);
        }
    }
}