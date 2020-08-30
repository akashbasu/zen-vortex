using System.Collections.Generic;

namespace RollyVortex
{
    public sealed class MetaStartState : BaseGameState
    {
        //Pre game UI
        protected override List<IInitializable> GetSteps(object[] args)
        {
            return new List<IInitializable>();
        }
    }
}