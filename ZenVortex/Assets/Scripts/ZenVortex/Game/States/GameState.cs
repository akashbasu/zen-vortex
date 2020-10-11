using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class GameState : BaseGameState
    {
        protected override void InstallDependencies()
        {
            DependencyRegistry.RegisterConcreteType<LevelDataManager>();
            DependencyRegistry.RegisterConcreteType<DeterministicRandomProvider>();
            
            DependencyRegistry.RegisterConcreteType<ObstacleDataManager>();
            DependencyRegistry.RegisterConcreteType<PowerupDataManager>();
            DependencyRegistry.RegisterConcreteType<ScoreDataManager>();
            
            DependencyRegistry.RegisterConcreteType<CollisionController>();
            DependencyRegistry.RegisterConcreteType<MovementController>();
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