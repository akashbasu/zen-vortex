using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class VibrationServiceController : IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityVibration<int>);
            _gameEventManager.Subscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityVibration);
            
            onComplete?.Invoke(this);
        }

        ~VibrationServiceController()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityVibration<int>);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityVibration);
        }

        private void PlayLowPriorityVibration<T>(object[] obj)
        {
            if(obj?.Length < 1) return;
            var data = (T) obj[0];
            if(data.Equals(default(T))) return;
            
            PlayVibrationForPriority(Priority.Low);
        }
        
        private void PlayMediumPriorityVibration(object[] obj)
        {
            PlayVibrationForPriority(Priority.Medium);
        }
        
        private void PlayHighPriorityVibration(object[] obj)
        {
            PlayVibrationForPriority(Priority.High);
        }

        private void PlayVibrationForPriority(Priority priority)
        {
            switch (priority)
            {
                case Priority.Low: PlayVibration(GameConstants.Device.Vibration.ShortVibrationTime);
                    break;
                case Priority.Medium: PlayVibration(GameConstants.Device.Vibration.LongVibrationTime);
                    break;
                case Priority.High: LeanTween.delayedCall(GameConstants.Device.Feedback.Delay, () => PlayVibration(GameConstants.Device.Vibration.LongVibrationTime))
                        .setRepeat(GameConstants.Device.Feedback.ChainCount);
                    break;
            }
        }

        private void PlayVibration(float time)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            vibrator.Call("vibrate", time);
#else
            Handheld.Vibrate();
#endif
        }
    }
    
    public static partial class GameConstants
    {
        internal static partial class Device
        {
            internal static class Vibration
            {
                public const float ShortVibrationTime = 0.25f;
                public const float LongVibrationTime = ShortVibrationTime * 2f;
            }
            
            internal static class Feedback
            {
                public const float Delay = 0.25f;
                public const int ChainCount = 3;
            }
        }
    }
}