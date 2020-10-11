using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class MetaEndState : BaseGameState
    {
        protected override void Configure()
        {
            DependencyRegistry.Register<ShareServiceController>();
        }
        
        protected override List<IInitializable> GetSteps(object[] args)
        {
            return new List<IInitializable>
            {
                new WaitForMetaEndComplete()
            };
        }
    }
}