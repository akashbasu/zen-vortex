using System.Collections.Generic;

namespace ZenVortex
{
    internal sealed class ResetState : BaseGameState
    {
        protected override void Configure() { }
        
        protected override List<IInitializable> GetSteps(object[] args) => new List<IInitializable>();
    }
}