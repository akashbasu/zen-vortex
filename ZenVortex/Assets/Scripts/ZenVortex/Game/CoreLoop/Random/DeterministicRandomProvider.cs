using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class DeterministicRandomProvider : IPostConstructable
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        [Dependency] private readonly LevelDataManager _levelDataManager;
        
        private Random _random;
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
        }
        
        public int Next() => _random.Next();
        public int Next(int min, int max) => _random.Next(min, max);
        public double NextNormalized() => _random.NextDouble();
        public int Next(IntRangedValue val) => _random.Next(val.Min, val.Max);
        public bool NextBool(float normalizedProbability) => NextNormalized() <= normalizedProbability;

        private void OnLevelStart(object[] obj)
        {
            _random = new Random(_levelDataManager.CurrentLevelData.Seed);
        }
    }
}