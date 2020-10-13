using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZenVortex.Editor
{
    internal class DataTools
    {
        [MenuItem("ZenVortex/DataTools/Level Data")]
        public static void CreateLevelData()
        {
            var newLevelData = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(newLevelData,
                Path.Combine(GameConstants.DataPaths.ResourcesBase, GameConstants.DataPaths.Resources.Levels, "Level_.asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("ZenVortex/DataTools/Obstacle Data")]
        public static void CreateObstacleData()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null) return;

            SerializeObstacleData(selectedObject);
        }
        
        [MenuItem("ZenVortex/DataTools/Powerups/Base Powerup Data")]
        public static void CreatBasePowerupData()
        {
            var outputPath = Path.Combine(GameConstants.DataPaths.ResourcesBase, GameConstants.DataPaths.Resources.Powerups,
                $"{Tags.Powerup}_X.asset").Replace('\\', '/');

            var doesAssetExist = AssetDatabase.GetAllAssetPaths().Any(x => string.Equals(outputPath, x));
            if(doesAssetExist) return;
            
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BasePowerup>(), outputPath);
            AssetDatabase.SaveAssets();
        }
        
        
        [MenuItem("ZenVortex/DataTools/Powerups/Timed Powerup Data")]
        public static void CreateTimedPowerupData()
        {
            var outputPath = Path.Combine(GameConstants.DataPaths.ResourcesBase, GameConstants.DataPaths.Resources.Powerups,
                $"{Tags.Powerup}_X.asset").Replace('\\', '/');

            var doesAssetExist = AssetDatabase.GetAllAssetPaths().Any(x => string.Equals(outputPath, x));
            if(doesAssetExist) return;
            
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TimedPowerup>(), outputPath);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("ZenVortex/DataTools/Obstacle Data", true)]
        public static bool CanCreateObstacleData()
        {
            return IsValidObstacle(Selection.activeGameObject);
        }

        [MenuItem("ZenVortex/DataTools/Export All Obstacle Data")]
        public static void ExportAllObstacleData()
        {
            var prefabSourcePath = Path.Combine("Assets", "Prefabs", "Obstacles", "DataOnlyPrefabs");
            var obstaclePrefabs = LoadAllPrefabs(prefabSourcePath);

            Debug.Log($"Serializing {obstaclePrefabs.Count} obstacle data");
            foreach (var obstaclePrefab in obstaclePrefabs) SerializeObstacleData(obstaclePrefab);
        }

        private static void SerializeObstacleData(GameObject source)
        {
            if(!IsValidObstacle(source)) return;

            var outputPath = Path.Combine(GameConstants.DataPaths.ResourcesBase, GameConstants.DataPaths.Resources.Obstacles,
                $"{Tags.Obstacle}_{source.name}.asset").Replace('\\', '/');

            var newObstacleData = AssetDatabase.GetAllAssetPaths().Any(x => string.Equals(outputPath, x))
                ? AssetDatabase.LoadAssetAtPath<ObstacleData>(outputPath)
                : ScriptableObject.CreateInstance<ObstacleData>();
                
            List<int> activated = new List<int>(), deactivated = new List<int>();
            foreach (Transform child in source.transform)
                if (child.gameObject.activeSelf) activated.Add(child.GetSiblingIndex());
                else deactivated.Add(child.GetSiblingIndex());
            activated.Sort();
            activated = activated.Distinct().ToList();
            deactivated.Sort();
            deactivated = deactivated.Distinct().ToList();
            newObstacleData.SerializeObstacleData(activated, deactivated);
            AssetDatabase.CreateAsset(newObstacleData, outputPath);
            AssetDatabase.SaveAssets();
        }

        private static bool IsValidObstacle(GameObject go)
        {
            return go != null && string.Equals(go.tag, Tags.Obstacle);
        }
        
        private static List<GameObject> LoadAllPrefabs(string path)
        {
            if (path != "")
            {
                if (path.EndsWith("/"))
                {
                    path = path.TrimEnd('/');
                }
            }

            var dirInfo = new DirectoryInfo(path);
            var fileInf = dirInfo.GetFiles("*.prefab");

            return (from fileInfo in fileInf
                select fileInfo.FullName.Replace(@"\", "/")
                into fullPath
                select "Assets" + fullPath.Replace(Application.dataPath, "")
                into assetPath
                select AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject))).OfType<GameObject>().ToList();
        }
    }
}