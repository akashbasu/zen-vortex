using System;
using UnityEngine;

namespace RollyVortex
{
    //Load level data here
    public class LevelManager : IInitializable
    {
        private static LevelData _levelData;

        public static LevelData LevelData => _levelData;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if(TryLoadLevelData())
            {
                onComplete?.Invoke(this);
            }
        }

        private bool TryLoadLevelData()
        {
            if (TryLoadDataForLevel(out _levelData)) return true;
            
            
            _levelData = ScriptableObject.CreateInstance<LevelData>();
            _levelData.SetDefault();

            return true;
        }

        private bool TryLoadDataForLevel(out LevelData dataForLevel)
        {
            //Load serialized data from Addressables
            dataForLevel = null;
            return false;
        }
    }
}