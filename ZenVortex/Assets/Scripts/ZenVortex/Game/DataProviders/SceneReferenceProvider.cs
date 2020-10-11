using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZenVortex
{
    [Serializable]
    internal class DirectoryEntry
    {
        [SerializeField] private GameObject _reference;
        [SerializeField] private string _tag;

        public string Tag => _tag;

        public GameObject Reference
        {
            get => _reference;
            set => _reference = value;
        }
    }

    internal interface ISceneReferenceProvider : IPostConstructable
    {
        bool TryGetEntry(string tag, out GameObject go);
    }

    internal class SceneReferenceProvider : MonoBehaviour, ISceneReferenceProvider
    {
        private readonly Dictionary<string, GameObject> _directory = new Dictionary<string, GameObject>();
        
        [SerializeField] private List<DirectoryEntry> _cachedEntries = new List<DirectoryEntry>();

        public void PostConstruct(params object[] args)
        {
            _directory.Clear();
            
            LoadCachedDirectory();
            FindDirectoryObjects();
        }

        public void Dispose()
        {
            _directory.Clear();
        }
        
        public bool TryGetEntry(string tag, out GameObject go)
        {
            go = null;
            
            if (_directory.ContainsKey(tag)) go = _directory[tag];

            if (go == null)
            {
                Debug.LogWarning(
                    $"[{nameof(SceneReferenceProvider)}] is searching for go with tag {tag}. This is slow and expensive!");
                MonoBehaviourUtils.SafeGetGoWithTag(tag, out go);
            }

            return go != null;
        }
        
        private void LoadCachedDirectory()
        {
            foreach (var cachedEntry in _cachedEntries) _directory[cachedEntry.Tag] = cachedEntry.Reference;
        }

        private void FindDirectoryObjects()
        {
            foreach (var entry in _directory.Where(entry => entry.Value == null))
                if (MonoBehaviourUtils.SafeGetGoWithTag(entry.Key, out var go))
                    _directory[entry.Key] = go;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = _cachedEntries.Count - 1; i >= 0; i--)
            {
                var entry = _cachedEntries[i];

                if (MonoBehaviourUtils.SafeGetGoWithTag(entry.Tag, out var foundObjectWithTag)) entry.Reference = foundObjectWithTag;
            }
        }
#endif
    }
}