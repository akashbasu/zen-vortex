using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenVortex
{
    internal interface IAudioServiceController : IPostConstructable
    {
        void PlayAudioForPriority(Priority priority);
    }
    
    internal class AudioServiceController : IAudioServiceController
    {
        private Vector3 _audioOrigin;
        private readonly Dictionary<Priority, AudioClip> _audioClips = new Dictionary<Priority, AudioClip>();
        
        public void PostConstruct(params object[] args)
        {
            _audioClips.Clear();
            LoadAudioResources();
            _audioOrigin = Camera.main.transform.position;
        }
        
        public void Dispose()
        {
            _audioClips.Clear();
        }
        
        public void PlayAudioForPriority(Priority priority)
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
                public static readonly string Audio = nameof(Audio);
            }
        }
    }
}