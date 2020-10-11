using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class BootState : BaseGameState
    {
        [Dependency] private readonly IUiServiceController _uiServiceController;
        
        protected override void InstallDependencies()
        {
            DependencyRegistry.RegisterInterface<IGameEventManager, GameEventManager>();
            
            DependencyRegistry.RegisterInterface<ISceneReferenceProvider, SceneReferenceProvider>();
            DependencyRegistry.RegisterInterface<IUiDataProvider, UiDataProvider>();
            
            DependencyRegistry.RegisterInterface<ITimeServiceController, TimeServiceController>();
            DependencyRegistry.RegisterInterface<IInputServiceController, InputServiceController>();
            DependencyRegistry.RegisterInterface<IVibrationServiceController, VibrationServiceController>();
            DependencyRegistry.RegisterInterface<IAudioServiceController, AudioServiceController>();
            DependencyRegistry.RegisterInterface<IUiServiceController, UiServiceController>();
        }
    }
}