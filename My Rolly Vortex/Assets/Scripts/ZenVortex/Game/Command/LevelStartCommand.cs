namespace ZenVortex
{
    internal sealed class LevelStartCommand : InitializableCommand
    {
        internal LevelStartCommand() : base(GameEvents.LevelEvents.Start) { }
    }
}