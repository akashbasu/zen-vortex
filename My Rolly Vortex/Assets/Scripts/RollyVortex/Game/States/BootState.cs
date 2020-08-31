using System.Collections.Generic;

namespace RollyVortex
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
                CreateMonoBehavior<MovementController>(),
            };

            return states;
        }
    }
}