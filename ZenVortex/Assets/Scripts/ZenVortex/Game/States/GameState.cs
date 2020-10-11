using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class GameState : BaseGameState
    {
        protected override void InstallDependencies()
        {
            DependencyRegistry.Register<LevelDataManager>();
            DependencyRegistry.Register<DeterministicRandomProvider>();
            
            DependencyRegistry.Register<ObstacleDataManager>();
            DependencyRegistry.Register<PowerupDataManager>();
            DependencyRegistry.Register<ScoreDataManager>();
            
            DependencyRegistry.Register<CollisionController>();
            DependencyRegistry.Register<MovementController>();
        }

        protected override Queue<IInitializable> GetSteps()
        {
            var queue = new Queue<IInitializable>();
            queue.Enqueue(new LevelStartEventCommand());
            queue.Enqueue(new WaitForGameLoopEnd());
            queue.Enqueue(new LevelEndEventCommand());

            return queue;
        }
    }
}