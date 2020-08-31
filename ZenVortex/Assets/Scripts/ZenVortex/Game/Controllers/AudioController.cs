using System;
using UnityEngine;

namespace ZenVortex
{
    internal class AudioController : IInitializable
    {
        private Vector3 _audioOrigin;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _audioOrigin = Camera.main.transform.position;
            
            GameEventManager.Subscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityAudio<int>);
            GameEventManager.Subscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityAudio);
            // GameEventManager.Subscribe(GameEvents.Gameplay.Pickup, PlayMediumPriorityAudio);
            
            onComplete?.Invoke(this);
        }

        ~AudioController()
        {
            GameEventManager.Unsubscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityAudio<int>);
            GameEventManager.Unsubscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityAudio);
            // GameEventManager.Unsubscribe(GameEvents.Gameplay.Pickup, PlayMediumPriorityAudio);
        }

        private void PlayLowPriorityAudio<T>(object[] obj)
        {
            if(obj?.Length < 1) return;
            var data = (T) obj[0];
            if(data.Equals(default(T))) return;
            
            PlayAudioForPriority(Priority.Low);
        }
        
        private void PlayMediumPriorityAudio(object[] obj)
        {
            PlayAudioForPriority(Priority.Medium);
        }
        
        private void PlayHighPriorityAudio(object[] obj)
        {
            PlayAudioForPriority(Priority.High);
        }

        private void PlayAudioForPriority(Priority priority)
        {
            switch (priority)
            {
                case Priority.Low: 
                case Priority.Medium:
                    PlayAudio(priority);
                    break;
                case Priority.High: LeanTween.delayedCall(GameConstants.Device.Feedback.Delay, () => PlayAudio(Priority.Medium))
                        .setRepeat(GameConstants.Device.Feedback.ChainCount);
                    break;
            }
        }

        private void PlayAudio(Priority priority)
        {
            if (AudioDataProvider.AudioClips.ContainsKey(priority))
            {
                AudioSource.PlayClipAtPoint(AudioDataProvider.AudioClips[priority], _audioOrigin);    
            }
            else
            {
                Debug.LogError($"[{nameof(AudioController)}] {nameof(PlayAudio)} failed to find audio clip for priority");
            }
        }
    }
}