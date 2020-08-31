using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RollyVortex
{
    internal class AudioDataProvider : IInitializable
    {
        private static Dictionary<Priority, AudioClip> _audioClips;
        public static Dictionary<Priority, AudioClip> AudioClips => _audioClips;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _audioClips = new Dictionary<Priority, AudioClip>();
            TryLoadAudioClips();
            
            onComplete?.Invoke(this);
        }

        private bool TryLoadAudioClips()
        {
            if (!LoadClipsFromDisk())
            {
                Debug.LogError($"[{nameof(AudioDataProvider)}]  {nameof(TryLoadAudioClips)} failed to find audio clips");
            }

            return true;
        }

        private bool LoadClipsFromDisk()
        {
            var audioAtPath = Resources.LoadAll<AudioClip>(GameConstants.DataPaths.Resources.Audio);
            foreach (var audioClip in audioAtPath)
            {
                if (Enum.TryParse<Priority>(audioClip.name, out var priority))
                {
                    _audioClips.Add(priority, audioClip);
                }
            }
            
            return _audioClips.Count > 0;
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