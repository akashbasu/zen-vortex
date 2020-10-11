using System.Collections.Generic;

namespace ZenVortex
{
    internal sealed class MetaEndState : BaseGameState
    {
        protected override void Configure() { }
        
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                MonoBehaviourUtils.CreateMonoBehaviorSingleton(typeof(ShareServiceController)) as IInitializable,
                new WaitForMetaEndComplete()
            };

            return states;
        }
    }
}