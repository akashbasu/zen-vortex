using System.Collections.Generic;

namespace ZenVortex
{
    internal sealed class BootState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                new GameEventManager(),
                CreateMonoBehavior<SceneReferenceProvider>(),
                new AudioDataProvider(),
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