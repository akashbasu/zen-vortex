using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class BootState : BaseGameState
    {
        [Dependency] private readonly UiServiceController _uiServiceController;
        
        protected override void InstallDependencies()
        {
            DependencyRegistry.RegisterConcreteType<GameEventManager>();
            
            DependencyRegistry.RegisterConcreteType<SceneReferenceProvider>();
            DependencyRegistry.RegisterConcreteType<UiDataProvider>();
            
            DependencyRegistry.RegisterConcreteType<TimeServiceController>();
            DependencyRegistry.RegisterConcreteType<InputServiceController>();
            DependencyRegistry.RegisterConcreteType<VibrationServiceController>();
            DependencyRegistry.RegisterConcreteType<AudioServiceController>();
            DependencyRegistry.RegisterConcreteType<UiServiceController>();
        }
    }
}