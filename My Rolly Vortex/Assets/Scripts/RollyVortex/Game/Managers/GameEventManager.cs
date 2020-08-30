using System;
using System.Collections.Generic;

namespace RollyVortex
{
    internal class GameEventManager : IInitializable
    {
        private static Dictionary<string, List<Action<object[]>>> _eventDirectory;

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _eventDirectory = new Dictionary<string, List<Action<object[]>>>();

            onComplete?.Invoke(this);
        }

        public static void Subscribe(string gameEvent, Action<object[]> callback)
        {
            if (_eventDirectory == null) return;

            if (_eventDirectory.ContainsKey(gameEvent))
                _eventDirectory[gameEvent].RemoveAll(x => x == callback);
            else
                _eventDirectory[gameEvent] = new List<Action<object[]>>();

            _eventDirectory[gameEvent].Add(callback);
        }

        public static void Unsubscribe(string gameEvent, Action<object[]> callback)
        {
            if (_eventDirectory == null) return;

            if (!_eventDirectory.ContainsKey(gameEvent)) return;

            var callbacks = _eventDirectory[gameEvent];
            callbacks?.RemoveAll(x => x == callback);
        }

        public static void Broadcast(string gameEvent, params object[] args)
        {
            if (_eventDirectory == null) return;

            if (!_eventDirectory.ContainsKey(gameEvent)) return;
            
            if(_eventDirectory[gameEvent] == null) return;

            var callbacks = _eventDirectory[gameEvent];
            var count = callbacks?.Count ?? 0;
            for (int i = count - 1; i >= 0; i--)
            {
                callbacks[i]?.Invoke(args);
            }
        }
    }
}