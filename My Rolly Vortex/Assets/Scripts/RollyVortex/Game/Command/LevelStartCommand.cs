namespace RollyVortex
{
    public sealed class LevelStartCommand : InitializableCommand
    {
        public LevelStartCommand() : base(GameEvents.LevelEvents.StartLevel) { }
    }
}