using UnityEngine;

namespace ZenVortex
{
    internal interface IVibrationServiceController
    {
        void PlayVibrationForPriority(Priority priority);
    }

#if UNITY_ANDROID || UNITY_IOS
    
    internal class VibrationServiceController : IVibrationServiceController
    {
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
            Handheld.Vibrate();
        }
    }
    
#else

    internal class NullVibrationServiceController : IVibrationServiceController
    {
        public void PlayVibrationForPriority(Priority priority) 
        { 
            Debug.Log($"[{nameof(NullVibrationServiceController)}] {nameof(PlayVibrationForPriority)} Vibration support does not exist for platform");
        }
    }
    
#endif
    
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