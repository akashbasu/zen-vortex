using System.Collections.Generic;

namespace RollyVortex
{
    internal sealed class MetaEndState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                CreateMonoBehavior<SocialManager>(),
                new WaitForMetaEndComplete()
            };

            return states;
        }
    }
}