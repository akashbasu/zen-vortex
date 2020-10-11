using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class MetaEndState : BaseGameState
    {
        protected override void InstallDependencies()
        {
            DependencyRegistry.Register<ShareServiceController>();
        }
        
        protected override Queue<IInitializable> GetSteps()
        {
            var queue = new Queue<IInitializable>();
            queue.Enqueue(new WaitForMetaEndComplete());

            return queue;
        }
    }
}