using System.Collections.Generic;

namespace ZenVortex
{
    internal sealed class MetaStartState : BaseGameState
    {
        protected override void Configure() { }
        
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