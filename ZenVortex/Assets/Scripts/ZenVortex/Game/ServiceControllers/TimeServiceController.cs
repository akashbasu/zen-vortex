using System;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface ITimeServiceController : IPostConstructable
    {
        float CurrentTimeScale { get; }
    }
    
    internal class TimeServiceController : ITimeServiceController
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;

        public float CurrentTimeScale => Time.timeScale;
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Time.OverrideScale, OnTimeScaleOverride);
            _gameEventManager.Subscribe(GameEvents.Time.IncrementScale, OnIncrementScale);
            _gameEventManager.Subscribe(GameEvents.Time.DecrementScale, OnDecrementScale);
            
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnReset);
            _gameEventManager.Subscribe(GameEvents.Gameplay.Reset, OnReset);
        }

        public void Dispose()
        {
            ResetTimeScaleOverride();
            
            _gameEventManager.Unsubscribe(GameEvents.Time.OverrideScale, OnTimeScaleOverride);
            _gameEventManager.Unsubscribe(GameEvents.Time.IncrementScale, OnIncrementScale);
            _gameEventManager.Unsubscribe(GameEvents.Time.DecrementScale, OnDecrementScale);
            
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnReset);
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Reset, OnReset);
        }

        private void OnReset(object[] obj)
        {
            ResetTimeScaleOverride();
        }
        
        private void OnTimeScaleOverride(object[] args)
        {
            if(args?.Length < 1 || !(args[0] is float scale)) return;

            SetScale(scale);
        }
        
        private void OnIncrementScale(object[] args)
        {
            if(args?.Length < 1 || !(args[0] is float increment)) return;

            SetScale(Time.timeScale + increment);
        }
        
        private void OnDecrementScale(object[] args)
        {
            if(args?.Length < 1 || !(args[0] is float decrement)) return;

            SetScale(Time.timeScale - decrement);
        }
        
        private void ResetTimeScaleOverride()
        {
            SetScale(GameConstants.Environment.DefaultTimeScale);
            Debug.Log($"[{nameof(TimeServiceController)}] {nameof(ResetTimeScaleOverride)}");
        }

        private void SetScale(float scale)
        {
            Time.timeScale = Math.Max(scale, GameConstants.Environment.MinimumTimeScale);
            Time.timeScale = Math.Min(scale, GameConstants.Environment.DefaultTimeScale);


            new TimeScaleUpdatedCommand().Execute();
            Debug.Log($"[{nameof(TimeServiceController)}] Time scale updated {Time.timeScale}");
        }
    }
    
    public static partial class GameConstants
    {
        internal static partial class Environment
        {
            public const float MinimumTimeScale = .35f;
            public const float DefaultTimeScale = 1f;
        }
    }
    
    internal partial class UiDataKeys
    {
        internal static partial class Time
        {
            public static readonly string Scale = $"{nameof(Time)}.{nameof(Scale)}";
        }
    }
}