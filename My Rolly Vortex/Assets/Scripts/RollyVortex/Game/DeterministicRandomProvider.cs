using System;

namespace RollyVortex
{
    public class DeterministicRandomProvider : IInitializable
    {
        private static Random _random;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            onComplete?.Invoke(this);
        }

        private void OnLevelStart(object[] obj)
        {
            _random = new Random(LevelDataProvider.LevelData.Seed);
        }

        public static int Next() => _random.Next();
        public static int Next(int min, int max) => _random.Next(min, max);
    }
}