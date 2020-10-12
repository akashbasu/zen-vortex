using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class GameState : BaseGameState
    {
        protected override void InstallDependencies()
        {
            DependencyRegistry.RegisterInterface<ILevelDataManager, LevelDataManager>();
            DependencyRegistry.RegisterInterface<IDeterministicRandomProvider, DeterministicRandomProvider>();
            
            DependencyRegistry.RegisterInterface<IObstacleDataManager, ObstacleDataManager>();
            DependencyRegistry.RegisterInterface<IPowerupDataManager, PowerupDataManager>();
            DependencyRegistry.RegisterInterface<IScoreDataManager, ScoreDataManager>();
            
            DependencyRegistry.RegisterConcreteType<BallMovement>();
            DependencyRegistry.RegisterConcreteType<ObstacleMovement>();
            DependencyRegistry.RegisterConcreteType<PowerupMovement>();
            DependencyRegistry.RegisterConcreteType<TubeMovement>();
            
            DependencyRegistry.RegisterInterface<ICollisionController, CollisionController>();
            DependencyRegistry.RegisterInterface<IMovementController, MovementController>();
        }

        protected override Queue<IInitializable> GetSteps()
        {
            var queue = new Queue<IInitializable>();
            
            queue.Enqueue(new WaitForGameEnd());

            return queue;
        }
    }
}