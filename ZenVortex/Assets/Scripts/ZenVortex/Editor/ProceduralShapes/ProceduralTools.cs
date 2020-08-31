using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZenVortex.Editor
{
    internal static class ProceduralTools
    {
        private const string MeshPath = "Assets/Models/";
        private const string MeshPostfix = "_Mesh.asset";

        [MenuItem("ZenVortex/ProceduralTools/Tube Mesh")]
        public static void MakeTube()
        {
            AddMesh(Selection.activeGameObject);
            Tube.Make(Selection.activeGameObject);
            AddRequiredComponents(Selection.activeGameObject);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("ZenVortex/ProceduralTools/Tube Mesh", true)]
        private static bool IsValidSelectionForMesh()
        {
            var activeGo = Selection.activeGameObject;
            var meshFilter = activeGo != null ? activeGo.GetComponent<MeshFilter>() : null;
            var sharedMesh = meshFilter != null ? meshFilter.sharedMesh : null;
            return activeGo != null && sharedMesh == null;
        }

        private static void AddMesh(GameObject gameObject)
        {
            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = gameObject.AddComponent<MeshFilter>();
                AssetDatabase.SaveAssets();
            }

            var mesh = filter.sharedMesh;
            if (mesh == null)
            {
                var path = Path.Combine(MeshPath, $"{gameObject.name}{MeshPostfix}");
                AssetDatabase.CreateAsset(new Mesh(), path);
                AssetDatabase.SaveAssets();
                mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            }

            filter.sharedMesh = mesh;
            EditorUtility.SetDirty(gameObject);
        }

        private static void AddRequiredComponents(GameObject gameObject)
        {
            if (gameObject.GetComponent<MeshRenderer>() != null) return;

            gameObject.AddComponent<MeshRenderer>();
            EditorUtility.SetDirty(gameObject);
        }
    }
}