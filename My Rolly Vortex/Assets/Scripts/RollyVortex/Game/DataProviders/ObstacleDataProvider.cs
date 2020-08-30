using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RollyVortex
{
    internal class ObstacleDataProvider : IInitializable
    {
        public static List<ObstacleData> ObstacleData { get; private set; }

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            ObstacleData = new List<ObstacleData>();
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
            ObstacleData.AddRange(Resources.LoadAll<ObstacleData>(GameConstants.DataPaths.Resources.Obstacles));
            return ObstacleData.Count > 0;
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