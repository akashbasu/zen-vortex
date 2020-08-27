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
            AssetDatabase.CreateAsset(newLevelData, Path.Combine("Assets", "Resources", GameConstants.DataPaths.LevelDataPath, "Level_.asset"));
            AssetDatabase.SaveAssets();
        }
        
        [MenuItem("RollyVortex/DataTools/Obstacle Data")]
        public static void CreateObstacleData()
        {
            var newLevelData = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(newLevelData, Path.Combine("Assets", "Resources", GameConstants.DataPaths.LevelDataPath, "Level_.asset"));
            AssetDatabase.SaveAssets();
        }
    }
}