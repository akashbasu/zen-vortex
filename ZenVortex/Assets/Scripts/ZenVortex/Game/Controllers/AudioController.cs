using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class AudioController : IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        [Dependency] private readonly AudioDataProvider _audioDataProvider;
        
        private Vector3 _audioOrigin;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _audioOrigin = Camera.main.transform.position;
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityAudio<int>);
            _gameEventManager.Subscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityAudio);
            // _gameEventManager.Subscribe(GameEvents.Gameplay.Pickup, PlayMediumPriorityAudio);
            
            onComplete?.Invoke(this);
        }

        ~AudioController()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityAudio<int>);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityAudio);
            // _gameEventManager.Unsubscribe(GameEvents.Gameplay.Pickup, PlayMediumPriorityAudio);
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
            if (_audioDataProvider.AudioClips.ContainsKey(priority))
            {
                AudioSource.PlayClipAtPoint(_audioDataProvider.AudioClips[priority], _audioOrigin);    
            }
            else
            {
                Debug.LogError($"[{nameof(AudioController)}] {nameof(PlayAudio)} failed to find audio clip for priority");
            }
        }
    }
}