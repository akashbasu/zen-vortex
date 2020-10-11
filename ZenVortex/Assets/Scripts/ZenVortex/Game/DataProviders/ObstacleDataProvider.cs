using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZenVortex
{
    internal class ObstacleDataProvider : IInitializable
    {
        public List<ObstacleData> ObstacleData => _obstacleData;
        
        private readonly List<ObstacleData> _obstacleData = new List<ObstacleData>();

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (TryLoadObstacleData()) onComplete?.Invoke(this);
        }

        private bool TryLoadObstacleData()
        {
            if (LoadDataFromDisk()) return true;

            Debug.LogError(
                $"{nameof(ObstacleDataProvider)} {nameof(TryLoadObstacleData)} failed to load data from disk!");
            return false;
        }

        private bool LoadDataFromDisk()
        {
            _obstacleData.AddRange(Resources.LoadAll<ObstacleData>(GameConstants.DataPaths.Resources.Obstacles));
            return _obstacleData.Count > 0;
        }
    }

    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public partial class Resources
            {
                public static readonly string Obstacles = Path.Combine("Data", "Obstacles");
            }
        }
    }
}