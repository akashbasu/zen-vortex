using System;
using System.Collections.Generic;

namespace RollyVortex
{
    public sealed class GameState : BaseGameState
    {
        protected override List<IInitializable> GetSteps()
        {
            var states = new List<IInitializable>();
            
            states.Add(new LevelManager());
            return states;
        }
    }
}