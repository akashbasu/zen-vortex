using System.Collections.Generic;

namespace ZenVortex
{
    internal sealed class MetaStartState : BaseGameState
    {
        protected override Queue<IInitializable> GetSteps()
        {
            var queue = new Queue<IInitializable>();
            queue.Enqueue(new WaitForMetaStartComplete());

            return queue;
        }
    }
}