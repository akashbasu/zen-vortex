using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PlayerState : BaseGameState
    {
        protected override void InstallDependencies()
        {
            DependencyRegistry.RegisterInterface<IPlayerLifeDataManager, PlayerLifeDataManager>();
            DependencyRegistry.RegisterInterface<ITimedInventoryDataManager, TimedInventoryDataManager>();
        }
    }
}