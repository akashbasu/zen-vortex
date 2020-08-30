namespace RollyVortex
{
    internal sealed class LevelEndCommand : InitializableCommand
    {
        internal LevelEndCommand() : base(GameEvents.LevelEvents.Stop) { }
    }
}