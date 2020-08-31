using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZenVortex
{
    internal class PowerupDataProvider : IInitializable
    {
        public static List<PowerupData> PowerupData { get; private set; }

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            PowerupData = new List<PowerupData>();
            if (TryLoadPowerupData()) onComplete?.Invoke(this);
        }

        private bool TryLoadPowerupData()
        {
            if (LoadDataFromDisk()) return true;

            Debug.LogError(
                $"{nameof(PowerupDataProvider)} {nameof(TryLoadPowerupData)} failed to load data from disk!");
            return false;
        }

        private bool LoadDataFromDisk()
        {
            PowerupData.AddRange(Resources.LoadAll<PowerupData>(GameConstants.DataPaths.Resources.Powerups));
            return PowerupData.Count > 0;
        }
    }

    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public partial class Resources
            {
                public static readonly string Powerups = Path.Combine("Data", "Powerups");
            }
        }
    }
}