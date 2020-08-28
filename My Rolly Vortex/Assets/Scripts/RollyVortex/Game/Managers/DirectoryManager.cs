using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
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

    internal class DirectoryManager : MonoBehaviour, IInitializable
    {
        private static Dictionary<string, GameObject> _directory;
        [SerializeField] private List<DirectoryEntry> _cachedEntries = new List<DirectoryEntry>();

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _directory = new Dictionary<string, GameObject>();

            LoadCachedDirectory();
            FindDirectoryObjects();

            onComplete?.Invoke(this);
        }

        public static bool TryGetEntry(string tag, out GameObject go)
        {
            go = null;
            if (_directory.ContainsKey(tag)) go = _directory[tag];

            if (go == null)
            {
                Debug.LogWarning(
                    $"[{nameof(DirectoryManager)}] is searching for go with tag. This is slow and expensive!");
                SafeGetGoWithTag(tag, out go);
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
                if (SafeGetGoWithTag(entry.Key, out var go))
                    _directory[entry.Key] = go;
        }

        private static bool SafeGetGoWithTag(string tag, out GameObject go)
        {
            try
            {
                go = GameObject.FindWithTag(tag);
                if (go != null) return true;
                throw new UnassignedReferenceException();
            }
            catch
            {
                Debug.LogError($"[{nameof(DirectoryManager)}] Failed to find Game object with tag {tag}");
                go = null;
                return false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = _cachedEntries.Count - 1; i >= 0; i--)
            {
                var entry = _cachedEntries[i];

                if (SafeGetGoWithTag(entry.Tag, out var foundObjectWithTag)) entry.Reference = foundObjectWithTag;
            }
        }
#endif
    }
}