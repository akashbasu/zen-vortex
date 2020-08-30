using System.Collections.Generic;

namespace RollyVortex
{
    internal sealed class PlayerState : BaseGameState
    {
        //Load player data here : History, High Score, Data on Disk
        protected override List<IInitializable> GetSteps(object[] args)
        {
            return new List<IInitializable>();
        }
    }
}