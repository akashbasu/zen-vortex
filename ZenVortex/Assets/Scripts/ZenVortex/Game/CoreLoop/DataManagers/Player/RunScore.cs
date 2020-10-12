using System;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace ZenVortex
{
    internal class RunScore
    {
        public int ObstaclesPassed;
        public int PowerupsPickedUp;
        public int ObstacleScore;
        public int PowerupScore;
        public int TotalScore;

        public void LoadHighScoreFromDisk(string baseFormatKey)
        {
            ObstaclesPassed = PlayerPrefs.GetInt(string.Format(baseFormatKey, nameof(ObstaclesPassed)), 0);
            PowerupsPickedUp = PlayerPrefs.GetInt(string.Format(baseFormatKey, nameof(PowerupsPickedUp)), 0);
            ObstacleScore = PlayerPrefs.GetInt(string.Format(baseFormatKey, nameof(ObstacleScore)), 0);
            PowerupScore = PlayerPrefs.GetInt(string.Format(baseFormatKey, nameof(PowerupScore)), 0);
            TotalScore = PlayerPrefs.GetInt(string.Format(baseFormatKey, nameof(TotalScore)), 0);
        }

        public void SaveHighScoreToDisk(string baseFormatKey)
        {
            PlayerPrefs.SetInt(string.Format(baseFormatKey, nameof(ObstaclesPassed)), ObstaclesPassed);
            PlayerPrefs.SetInt(string.Format(baseFormatKey, nameof(PowerupsPickedUp)), PowerupsPickedUp);
            PlayerPrefs.SetInt(string.Format(baseFormatKey, nameof(ObstacleScore)), ObstacleScore);
            PlayerPrefs.SetInt(string.Format(baseFormatKey, nameof(PowerupScore)), PowerupScore);
            PlayerPrefs.SetInt(string.Format(baseFormatKey, nameof(TotalScore)), TotalScore);
        }

        public void UpdateMaxScore(RunScore other)
        {
            ObstaclesPassed = Math.Max(ObstaclesPassed, other.ObstaclesPassed);
            PowerupsPickedUp = Math.Max(PowerupsPickedUp, other.PowerupsPickedUp);
            ObstacleScore = Math.Max(ObstacleScore, other.ObstacleScore);
            PowerupScore = Math.Max(PowerupScore, other.PowerupScore);
            TotalScore = Math.Max(TotalScore, other.TotalScore);
        }
    }
}