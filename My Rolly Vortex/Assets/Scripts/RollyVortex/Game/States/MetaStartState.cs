using System.Collections.Generic;

namespace RollyVortex
{
    internal sealed class MetaStartState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                new WaitForMetaStartComplete()
            };

            return states;
        }
    }
}