namespace RollyVortex
{
    public static class GameEvents
    {
        public static class GameStateEvents
        {
            public const string Start = nameof(Start);
            public const string End = nameof(End);
        }
        
        public static class LevelEvents
        {
            public const string StartLevel = nameof(StartLevel);
            public const string StopLevel = nameof(StopLevel);    
        }

        public static class Gameplay
        {
            public const string CollisionStart = nameof(CollisionStart);
            public const string CollisionStay = nameof(CollisionStay);
            public const string CollisionEnd = nameof(CollisionEnd);
        }
    }
}