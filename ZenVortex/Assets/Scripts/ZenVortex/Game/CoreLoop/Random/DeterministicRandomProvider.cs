using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IDeterministicRandomProvider : IPostConstructable
    {
        int Next();
        int Next(int min, int max);
        double NextNormalized();
        int Next(IntRangedValue val);
        bool NextBool(float normalizedProbability);
    }
    
    internal class DeterministicRandomProvider : IDeterministicRandomProvider
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly ILevelDataManager _levelDataManager;
        
        private Random _random;
        
        public void PostConstruct(params object[] args)
        {
            _gameEventManager.Subscribe(GameEvents.Gameplay.Start, OnGameStart);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.Gameplay.Start, OnGameStart);
        }
        
        public int Next() => _random.Next();
        public int Next(int min, int max) => _random.Next(min, max);
        public double NextNormalized() => _random.NextDouble();
        public int Next(IntRangedValue val) => _random.Next(val.Min, val.Max);
        public bool NextBool(float normalizedProbability) => NextNormalized() <= normalizedProbability;

        private void OnGameStart(object[] obj)
        {
            _random = new Random(_levelDataManager.CurrentLevelData.Seed);
        }
    }
}