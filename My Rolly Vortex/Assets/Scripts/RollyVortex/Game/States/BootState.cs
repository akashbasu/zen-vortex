using System.Collections.Generic;
using UnityEngine;

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
                CreateMonoBehavior<MovementController>(),
                new InputController(),
                new UiDataProvider(),
                new UiController()
            };

            return states;
        }
    }
}