using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PlayerState : BaseGameState
    {
        protected override void Configure()
        {
            DependencyRegistry.Register<PlayerDataManager>();
        }
        
        protected override List<IInitializable> GetSteps(object[] args) => new List<IInitializable>();
    }
}