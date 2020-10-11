using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IPlayerDataManager : IPostConstructable
    {
        int LastRunScore { get; }
        int LifeCount { get; }
    }
    
    internal class PlayerDataManager : IPlayerDataManager
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly IUiDataProvider _uiDataProvider;

        private int _highestScore;
        private int _lastRunScore;
        private int _livesEarnedInRun;

        public int LastRunScore => _lastRunScore;
        public int LifeCount => _livesEarnedInRun;
        
        public void PostConstruct(params object[] args)
        {
            LoadPlayerData();
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.ScoreUpdated, OnScoreUpdated);
            _gameEventManager.Subscribe(GameEvents.Gameplay.EarnedLife, OnLifeEarned);
            _gameEventManager.Subscribe(GameEvents.Gameplay.ConsumeLife, OnLifeLost);
            _gameEventManager.Subscribe(GameEvents.Gameplay.End, UpdateLastRunScore);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.End, UpdateLastRunScore);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.EarnedLife, OnLifeEarned);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.ConsumeLife, OnLifeLost);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.ScoreUpdated, OnScoreUpdated);
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
                new EventCommand(GameEvents.Gameplay.End).Execute();
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
            _uiDataProvider.UpdateData(UiDataKeys.Player.LastScore, _lastRunScore);
            _uiDataProvider.UpdateData(UiDataKeys.Player.HighScore, _highestScore);
            _uiDataProvider.UpdateData(UiDataKeys.Player.Lives, _livesEarnedInRun);
        }

        private void StorePlayerData()
        {
            if (_highestScore != PlayerPrefs.GetInt(GameConstants.PlayerData.LastScore, 0))
            {
                new EventCommand(GameEvents.Gameplay.HighScore);
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