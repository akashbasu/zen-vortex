namespace ZenVortex
{
    internal static class GameEvents
    {
        internal static class StateMachineEvents
        {
            public static readonly string Start = $"{nameof(StateMachineEvents)}.{nameof(Start)}";
            public static readonly string End = $"{nameof(StateMachineEvents)}.{nameof(End)}";
        }

        internal static class Gameplay
        {
            public static readonly string Start = $"{nameof(Gameplay)}.{nameof(Start)}";
            public static readonly string Stop = $"{nameof(Gameplay)}.{nameof(Stop)}";
            public static readonly string Reset = $"{nameof(Gameplay)}.{nameof(Reset)}";
        }

        internal static class Obstacle
        {
            public static readonly string Crossed = $"{nameof(Obstacle)}.{nameof(Crossed)}";
            public static readonly string Collision = $"{nameof(Obstacle)}.{nameof(Collision)}";
        }
        
        internal static class Powerup
        {
            public static readonly string Collect = $"{nameof(Powerup)}.{nameof(Collect)}";
            public static readonly string EarnedLife = $"{nameof(Powerup)}.{nameof(EarnedLife)}";
            public static readonly string OverrideSize = $"{nameof(Powerup)}.{nameof(OverrideSize)}";
            public static readonly string OverrideTimeScale = $"{nameof(Powerup)}.{nameof(OverrideTimeScale)}";
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