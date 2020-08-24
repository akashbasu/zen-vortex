namespace RollyVortex
{
    public sealed class LevelEndCommand : InitializableCommand
    {
        public LevelEndCommand() : base(GameEvents.LevelEvents.StopLevel)
        {
        }
    }
}