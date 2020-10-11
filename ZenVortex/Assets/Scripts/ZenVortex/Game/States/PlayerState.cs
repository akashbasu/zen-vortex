using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PlayerState : BaseGameState
    {
        protected override void InstallDependencies()
        {
            DependencyRegistry.RegisterConcreteType<PlayerDataManager>();
        }
    }
}