using System.Collections.Generic;

namespace RollyVortex
{
    public sealed class BootState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>();
            
            states.Add(new GameEventManager());
            foreach (var monoBehaviorBootables in args) states.Add(monoBehaviorBootables as IInitializable);
            states.Add(new InputController());
            states.Add(new UiController());
            
            return states;
        }
    }
}