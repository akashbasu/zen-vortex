using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RollyVortex
{
    internal class LevelDataProvider : IInitializable
    {
        private static List<LevelData> _levelData;
        private static int _currentLevel = -1;

        internal static LevelData LevelData => _levelData[_currentLevel];

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
            _levelData.AddRange(Resources.LoadAll<LevelData>(GameConstants.DataPaths.Resources.Levels));
            return _levelData.Count > 0;
        }

        private static bool LoadDataForLevel(int level)
        {
            _currentLevel = Mathf.Clamp(level, 0, _levelData.Count - 1);
            return true;
        }
    }

    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public partial class Resources
            {
                public static readonly string Levels = Path.Combine("Data", "Levels");
            }
        }
    }
}