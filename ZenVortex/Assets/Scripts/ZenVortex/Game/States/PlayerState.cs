using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class PlayerState : BaseGameState
    {
        [Dependency] private readonly PlayerDataProvider _playerDataProvider;
        
        protected override void Configure()
        {
            DependencyRegistry.Register<PlayerDataProvider>();
        }
        
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var steps = new List<IInitializable>
            {
                _playerDataProvider
            };

            return steps;
        }
    }
}