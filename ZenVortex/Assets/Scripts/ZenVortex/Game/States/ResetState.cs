using System.Collections.Generic;

namespace ZenVortex
{
    //Clean up here
    internal sealed class ResetState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            return new List<IInitializable>();
        }
    }
}