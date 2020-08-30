using System;

namespace RollyVortex
{
    public class DeterministicRandomProvider : IInitializable
    {
        private static Random _random;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Subscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
            
            onComplete?.Invoke(this);
        }

        private static void OnLevelStop(object[] obj)
        {
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Start, OnLevelStart);
            GameEventManager.Unsubscribe(GameEvents.LevelEvents.Stop, OnLevelStop);
        }

        private static void OnLevelStart(object[] obj)
        {
            _random = new Random(LevelDataProvider.LevelData.Seed);
        }

        public static int Next() => _random.Next();
        public static int Next(int min, int max) => _random.Next(min, max);
        public static int Next(IntRangedValue val) => _random.Next(val.Min, val.Max);
    }
}