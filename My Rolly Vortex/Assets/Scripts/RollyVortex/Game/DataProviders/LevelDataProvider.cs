using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class LevelDataProvider : IInitializable
    {
        private static List<LevelData> _levelData;
        private static int _currentLevel = -1;

        public static LevelData LevelData => _levelData[_currentLevel];

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _levelData = new List<LevelData>();
            if (TryLoadLevelData()) onComplete?.Invoke(this);
        }

        private bool TryLoadLevelData()
        {
            if (LoadDataFromDisk() &&
                LoadDataForLevel(_currentLevel)) return true;

            _levelData.Add(ScriptableObject.CreateInstance<LevelData>());
            return true;
        }

        private bool LoadDataFromDisk()
        {
            _levelData.AddRange(Resources.LoadAll<LevelData>(GameConstants.DataPaths.LevelDataPath));
            return _levelData.Count > 0;
        }

        private static bool LoadDataForLevel(int level)
        {
            _currentLevel = Mathf.Clamp(level, 0, _levelData.Count - 1);
            return true;
        }
    }
}