using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class BootState : BaseGameState
    {
        [Dependency] private readonly SceneReferenceProvider _sceneReferenceProvider;
        [Dependency] private readonly TimeServiceController _timeServiceController;
        [Dependency] private readonly InputServiceController _inputServiceController;
        [Dependency] private readonly VibrationServiceController _vibrationServiceController;
        [Dependency] private readonly AudioServiceController _audioServiceController;
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

        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                _sceneReferenceProvider,
                
                _timeServiceController,
                _inputServiceController,
                _vibrationServiceController,
                _audioServiceController,
                _uiServiceController
            };

            return states;
        }
    }
}