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
            public static string CrossedObstacle = $"{nameof(Gameplay)}.{nameof(CrossedObstacle)}";
            public static string Reset = $"{nameof(Gameplay)}.{nameof(Reset)}";
            public static string ScoreUpdated = $"{nameof(Gameplay)}.{nameof(ScoreUpdated)}";
        }

        internal static class Collisions
        {
            public static readonly string Start = $"{nameof(Collisions)}.{nameof(Start)}";
            public static readonly string Stay = $"{nameof(Collisions)}.{nameof(Stay)}";
            public static readonly string End = $"{nameof(Collisions)}.{nameof(End)}";
        }
        
        internal static class Application
        {
            public static readonly string Share = $"{nameof(Application)}.{nameof(Share)}";
            public static readonly string Contact = $"{nameof(Application)}.{nameof(Contact)}";
        }
    }
}