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
            DependencyRegistry.RegisterInterface<IUiServiceController, UiServiceController>();
            
            InstallPlatformDependencies();
            
            DependencyRegistry.RegisterInterface<IAudioServiceController, AudioServiceController>();
        }

        private void InstallPlatformDependencies()
        {
#if UNITY_ANDROID || UNITY_IOS
            DependencyRegistry.RegisterInterface<IVibrationServiceController, VibrationServiceController>();
#else
            DependencyRegistry.RegisterInterface<IVibrationServiceController, NullVibrationServiceController>();
#endif
        }
    }
}