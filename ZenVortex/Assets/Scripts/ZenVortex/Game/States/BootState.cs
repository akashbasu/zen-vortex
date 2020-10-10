using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class BootState : BaseGameState
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly AudioDataProvider _audioDataProvider;

        protected override void ConfigureState()
        {
            DependencyRegistry.Register<IGameEventManager, GameEventManager>();
            DependencyRegistry.Register<IGameEventManager, GameEventManager>();
        }

        protected override List<IInitializable> GetSteps(object[] args)
        {
            DependencyInjection.Inject<BootState>(this);
            
            var states = new List<IInitializable>
            {
                CreateMonoBehavior<SceneReferenceProvider>(),
                _gameEventManager,
                _audioDataProvider,
                new UiDataProvider(),
                
                new TimeController(),
                new InputController(),
                new VibrationController(),
                new AudioController(),
                new UiController(),
                new CollisionController(),
                CreateMonoBehavior<MovementController>(),
            };

            return states;
        }
    }
}