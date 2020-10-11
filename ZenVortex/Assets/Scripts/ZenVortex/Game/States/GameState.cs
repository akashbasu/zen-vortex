using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class GameState : BaseGameState
    {
        [Dependency] private readonly CollisionController _collisionController;
        [Dependency] private readonly MovementController _movementController;
        
        [Dependency] private readonly LevelDataProvider _levelDataProvider;
        [Dependency] private readonly DeterministicRandomProvider _deterministicRandomProvider;
        [Dependency] private readonly ObstacleDataProvider _obstacleDataProvider;
        [Dependency] private readonly PowerupDataProvider _powerupDataProvider;
        
        [Dependency] private readonly ObstacleManager _obstacleManager;
        [Dependency] private readonly PowerupManager _powerupManager;
        [Dependency] private readonly ScoreManager _scoreManager;
        
        protected override void Configure()
        {
            DependencyRegistry.Register<LevelDataProvider>();
            DependencyRegistry.Register<DeterministicRandomProvider>();
            DependencyRegistry.Register<ObstacleDataProvider>();
            DependencyRegistry.Register<PowerupDataProvider>();
            
            DependencyRegistry.Register<CollisionController>();
            DependencyRegistry.Register<MovementController>();
            
            DependencyRegistry.Register<ObstacleManager>();
            DependencyRegistry.Register<PowerupManager>();
            DependencyRegistry.Register<ScoreManager>();
        }

        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                _levelDataProvider,
                _deterministicRandomProvider,
                _obstacleDataProvider,
                _powerupDataProvider,
                
                _collisionController,
                _movementController,

                _obstacleManager,
                _powerupManager,
                _scoreManager,

                new LevelStartEventCommand(),
                new WaitForGameLoopEnd(),
                new LevelEndEventCommand()
            };


            return states;
        }
    }
}