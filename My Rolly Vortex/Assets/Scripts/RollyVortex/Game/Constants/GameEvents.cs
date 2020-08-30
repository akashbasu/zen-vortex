namespace RollyVortex
{
    internal static class GameEvents
    {
        internal static class GameStateEvents
        {
            public static readonly string Start = $"{nameof(GameStateEvents)}.{nameof(Start)}";
            public static readonly string End = $"{nameof(GameStateEvents)}.{nameof(End)}";
        }

        internal static class LevelEvents
        {
            public static readonly string Start = $"{nameof(LevelEvents)}.{nameof(Start)}";
            public static readonly string Stop = $"{nameof(LevelEvents)}.{nameof(Stop)}";
        }

        internal static class Gameplay
        {
            public static string Start = $"{nameof(Gameplay)}.{nameof(Start)}";
            public static string End = $"{nameof(Gameplay)}.{nameof(End)}";
        }

        internal static class Collisions
        {
            public static readonly string Start = $"{nameof(Collisions)}.{nameof(Start)}";
            public static readonly string Stay = $"{nameof(Collisions)}.{nameof(Stay)}";
            public static readonly string End = $"{nameof(Collisions)}.{nameof(End)}";
        }
    }
}