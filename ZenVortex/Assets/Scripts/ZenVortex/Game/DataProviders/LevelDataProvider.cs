using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZenVortex
{
    internal class LevelDataProvider : IInitializable
    {
        private readonly List<LevelData> _levelData = new List<LevelData>();
        private int _currentLevel = -1;

        internal LevelData LevelData => _levelData[_currentLevel];

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (TryLoadLevelData()) onComplete?.Invoke(this);
        }

        private bool TryLoadLevelData()
        {
            if (LoadDataFromDisk() && LoadDataForLevel(_currentLevel)) return true;

            _levelData.Add(ScriptableObject.CreateInstance<LevelData>());
            return true;
        }

        private bool LoadDataFromDisk()
        {
            _levelData.AddRange(Resources.LoadAll<LevelData>(GameConstants.DataPaths.Resources.Levels));
            return _levelData.Count > 0;
        }

        private bool LoadDataForLevel(int level)
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