using System.Collections.Generic;

namespace ZenVortex
{
    internal sealed class MetaEndState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                MonoBehaviourUtils.CreateMonoBehaviorSingleton<SocialManager>(),
                new WaitForMetaEndComplete()
            };

            return states;
        }
    }
}