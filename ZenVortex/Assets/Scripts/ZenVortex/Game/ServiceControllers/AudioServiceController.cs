using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class AudioServiceController : IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        
        private Vector3 _audioOrigin;
        private readonly Dictionary<Priority, AudioClip> _audioClips = new Dictionary<Priority, AudioClip>();
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            LoadAudioResources();
            _audioOrigin = Camera.main.transform.position;
            
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityAudio<int>);
            _gameEventManager.Subscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityAudio);
            // _gameEventManager.Subscribe(GameEvents.Gameplay.Pickup, PlayMediumPriorityAudio);
            
            onComplete?.Invoke(this);
        }

        ~AudioServiceController()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.ScoreUpdated, PlayLowPriorityAudio<int>);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.HighScore, PlayHighPriorityAudio);
            // _gameEventManager.Unsubscribe(GameEvents.Gameplay.Pickup, PlayMediumPriorityAudio);
        }
        
        private void LoadAudioResources()
        {
            var resourceLoader = new ResourcesLoader<AudioClip>(GameConstants.DataPaths.Resources.Audio);
            if (!resourceLoader.TryLoadData(out var resources))
            {
                Debug.LogException(new Exception($"[{nameof(AudioServiceController)}] failed to load data."));
            }
            
            foreach (var audioClip in resources)
            {
                if (Enum.TryParse<Priority>(audioClip.name, out var priority))
                {
                    _audioClips.Add(priority, audioClip);
                }
            }
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
            if (_audioClips.ContainsKey(priority))
            {
                AudioSource.PlayClipAtPoint(_audioClips[priority], _audioOrigin);    
            }
            else
            {
                Debug.LogError($"[{nameof(AudioServiceController)}] {nameof(PlayAudio)} failed to find audio clip for priority");
            }
        }
    }
    
    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public partial class Resources
            {
                public static readonly string Audio = Path.Combine("Data", "Audio");
            }
        }
    }
}