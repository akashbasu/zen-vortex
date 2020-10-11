using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class MetaEndState : BaseGameState
    {
        [Dependency] private readonly ShareServiceController _shareServiceController;
        
        protected override void Configure()
        {
            DependencyRegistry.Register<ShareServiceController>();
        }
        
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                _shareServiceController,
                new WaitForMetaEndComplete()
            };

            return states;
        }
    }
}