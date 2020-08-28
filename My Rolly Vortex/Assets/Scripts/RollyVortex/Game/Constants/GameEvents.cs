namespace RollyVortex
{
    public static class GameEvents
    {
        public static class GameStateEvents
        {
            public static readonly string Start = $"{nameof(GameStateEvents)}.{nameof(Start)}";
            public static readonly string End = $"{nameof(GameStateEvents)}.{nameof(End)}";
        }

        public static class LevelEvents
        {
            public static readonly string Start = $"{nameof(LevelEvents)}.{nameof(Start)}";
            public static readonly string Stop = $"{nameof(LevelEvents)}.{nameof(Stop)}";
        }

        public static class Gameplay
        {
        }

        public static class Collisions
        {
            public static readonly string Start = $"{nameof(Collisions)}.{nameof(Start)}";
            public static readonly string Stay = $"{nameof(Collisions)}.{nameof(Stay)}";
            public static readonly string End = $"{nameof(Collisions)}.{nameof(End)}";
        }
    }
}