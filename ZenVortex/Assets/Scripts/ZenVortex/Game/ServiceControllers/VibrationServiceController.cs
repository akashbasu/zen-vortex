using UnityEngine;

namespace ZenVortex
{
    internal interface IVibrationServiceController
    {
        void PlayVibrationForPriority(Priority priority);
    }
    
    internal class VibrationServiceController : IVibrationServiceController
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif

        public void PlayVibrationForPriority(Priority priority)
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
// #if UNITY_ANDROID && !UNITY_EDITOR
//             vibrator.Call("vibrate", time);
// #else
            Handheld.Vibrate();
// #endif
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