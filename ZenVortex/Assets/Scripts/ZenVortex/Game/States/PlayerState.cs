using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PlayerState : BaseGameState
    {
        [Dependency] private readonly PlayerDataManager _playerDataManager;
        
        protected override void Configure()
        {
            DependencyRegistry.Register<PlayerDataManager>();
        }
        
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var steps = new List<IInitializable>
            {
                _playerDataManager
            };

            return steps;
        }
    }
}