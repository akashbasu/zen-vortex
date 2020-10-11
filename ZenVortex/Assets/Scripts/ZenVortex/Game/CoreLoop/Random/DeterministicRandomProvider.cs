using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class DeterministicRandomProvider : IInitializable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        [Dependency] private readonly LevelDataManager _levelDataManager;
        
        private Random _random;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
            
            onComplete?.Invoke(this);
        }
        
        public int Next() => _random.Next();
        public int Next(int min, int max) => _random.Next(min, max);
        public double NextNormalized() => _random.NextDouble();
        public int Next(IntRangedValue val) => _random.Next(val.Min, val.Max);
        public bool NextBool(float normalizedProbability) => NextNormalized() <= normalizedProbability;

        private void OnLevelStop(object[] obj)
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
        }

        private void OnLevelStart(object[] obj)
        {
            _random = new Random(_levelDataManager.CurrentLevelData.Seed);
        }
    }
}