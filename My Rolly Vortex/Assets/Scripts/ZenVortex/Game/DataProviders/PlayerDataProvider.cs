using System;
using UnityEngine;

namespace ZenVortex
{
    internal class PlayerDataProvider : IInitializable
    {
        private int _highestScore;
        private static int _lastRunScore;
        private static int _livesEarnedInRun;

        public static int LastRunScore => _lastRunScore;
        public static int LifeCount => _livesEarnedInRun;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            LoadPlayerData();
            
            GameEventManager.Subscribe(GameEvents.Gameplay.ScoreUpdated, OnScoreUpdated);
            GameEventManager.Subscribe(GameEvents.Gameplay.EarnedLife, OnLifeEarned);
            GameEventManager.Subscribe(GameEvents.Gameplay.ConsumeLife, OnLifeLost);
            GameEventManager.Subscribe(GameEvents.Gameplay.End, UpdateLastRunScore);
            
            onComplete?.Invoke(this);
        }
        
        ~PlayerDataProvider()
        {
            GameEventManager.Unsubscribe(GameEvents.Gameplay.End, UpdateLastRunScore);
            GameEventManager.Unsubscribe(GameEvents.Gameplay.EarnedLife, OnLifeEarned);
            GameEventManager.Unsubscribe(GameEvents.Gameplay.ConsumeLife, OnLifeLost);
            GameEventManager.Unsubscribe(GameEvents.Gameplay.ScoreUpdated, OnScoreUpdated);
        }
        
        private void OnScoreUpdated(object[] obj)
        {
            if(obj?.Length < 1) return;

            _lastRunScore = (int) obj[0];
            _highestScore = Math.Max(_lastRunScore, _highestScore);
            UpdateUiData();
        }
        
        private void OnLifeLost(object[] obj)
        {
            _livesEarnedInRun--;
            if (_livesEarnedInRun < 0)
            {
                new Command(GameEvents.Gameplay.End).Execute();
            }
        }

        private void LoadPlayerData()
        {
             _lastRunScore = PlayerPrefs.GetInt(GameConstants.PlayerData.LastScore, 0);
             _highestScore = PlayerPrefs.GetInt(GameConstants.PlayerData.HighScore, 0);
        }
        
        private void UpdateLastRunScore(object[] obj)
        {
            _highestScore = Math.Max(_lastRunScore, _highestScore);
            UpdateUiData();
            StorePlayerData();
        }
        
        private void OnLifeEarned(object[] obj)
        {
            _livesEarnedInRun++;
            UpdateUiData();
        }

        private void UpdateUiData()
        {
            UiDataProvider.UpdateData(UiDataKeys.Player.LastScore, _lastRunScore);
            UiDataProvider.UpdateData(UiDataKeys.Player.HighScore, _highestScore);
            UiDataProvider.UpdateData(UiDataKeys.Player.Lives, _livesEarnedInRun);
        }

        private void StorePlayerData()
        {
            if (_highestScore != PlayerPrefs.GetInt(GameConstants.PlayerData.LastScore, 0))
            {
                new Command(GameEvents.Gameplay.HighScore);
            }
            
            PlayerPrefs.SetInt(GameConstants.PlayerData.LastScore, _lastRunScore);
            PlayerPrefs.SetInt(GameConstants.PlayerData.HighScore, _highestScore);
        }
    }

    public static partial class GameConstants
    {
        internal static partial class PlayerData
        {
            public static readonly string HighScore = $"{nameof(PlayerData)}.{nameof(HighScore)}";
            public static readonly string LastScore = $"{nameof(PlayerData)}.{nameof(LastScore)}";
        }
    }
    
    internal static partial class UiDataKeys
    {
        internal static partial class Player
        {
            public static readonly string LastScore = $"{nameof(Player)}.{nameof(LastScore)}";
            public static readonly string HighScore = $"{nameof(Player)}.{nameof(HighScore)}";
            public static readonly string Lives = $"{nameof(Player)}.{nameof(Lives)}";
        }
    }
}