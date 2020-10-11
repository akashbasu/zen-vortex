using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class GameState : BaseGameState
    {
        protected override void Configure()
        {
            DependencyRegistry.Register<LevelDataManager>();
            DependencyRegistry.Register<DeterministicRandomProvider>();
            
            DependencyRegistry.Register<ObstacleDataManager>();
            DependencyRegistry.Register<PowerupDataManager>();
            DependencyRegistry.Register<ScoreDataManager>();
            
            DependencyRegistry.Register<CollisionController>();
            DependencyRegistry.Register<MovementController>();
        }

        protected override List<IInitializable> GetSteps(object[] args)
        {
            return new List<IInitializable>
            {
                new LevelStartEventCommand(),
                new WaitForGameLoopEnd(),
                new LevelEndEventCommand()
            };
        }
    }
}