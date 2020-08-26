using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public sealed class BootState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>();
            
            
            states.Add(new GameEventManager());
            foreach (var monoBehaviorBootables in args)
            {
                var initializable = monoBehaviorBootables as IInitializable;
                if (initializable == null)
                {
                    Debug.LogError($"[{nameof(BootState)}] {nameof(GetSteps)} Invalid reference to Monobehavior initializable");
                    continue;
                }
                states.Add(initializable);
            }
            states.Add(new InputController());
            states.Add(new UiController());

            return states;
        }
    }
}