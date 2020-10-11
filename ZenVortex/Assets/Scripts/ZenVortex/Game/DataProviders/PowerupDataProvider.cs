using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZenVortex
{
    internal class PowerupDataProvider : IInitializable
    {
        public List<PowerupData> PowerupData => _powerupData;
        
        private readonly List<PowerupData> _powerupData = new List<PowerupData>();

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
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
            _powerupData.AddRange(Resources.LoadAll<PowerupData>(GameConstants.DataPaths.Resources.Powerups));
            return _powerupData.Count > 0;
        }
    }

    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public static partial class Resources
            {
                public static readonly string Powerups = Path.Combine("Data", "Powerups");
            }
        }
    }
}