using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class ObstacleDataProvider : IInitializable
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
            ObstacleData.AddRange(Resources.LoadAll<ObstacleData>(GameConstants.DataPaths.ObstacleDataPath));
            return ObstacleData.Count > 0;
        }
    }
}