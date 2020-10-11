using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class BootState : BaseGameState
    {
        [Dependency] private readonly UiServiceController _uiServiceController;
        
        protected override void Configure()
        {
            DependencyRegistry.Register<GameEventManager>();
            
            DependencyRegistry.Register<SceneReferenceProvider>();
            DependencyRegistry.Register<UiDataProvider>();
            
            DependencyRegistry.Register<TimeServiceController>();
            DependencyRegistry.Register<InputServiceController>();
            DependencyRegistry.Register<VibrationServiceController>();
            DependencyRegistry.Register<AudioServiceController>();
            DependencyRegistry.Register<UiServiceController>();
        }

        protected override List<IInitializable> GetSteps(object[] args) => new List<IInitializable>();
    }
}