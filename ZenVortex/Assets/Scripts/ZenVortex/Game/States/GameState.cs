using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal sealed class GameState : BaseGameState
    {
        [Dependency] private readonly LevelDataManager _levelDataManager;
        [Dependency] private readonly DeterministicRandomProvider _deterministicRandomProvider;
        
        [Dependency] private readonly CollisionController _collisionController;
        [Dependency] private readonly MovementController _movementController;
        
        [Dependency] private readonly ObstacleDataManager _obstacleDataManager;
        [Dependency] private readonly PowerupDataManager _powerupDataManager;
        [Dependency] private readonly ScoreManager _scoreManager;
        
        protected override void Configure()
        {
            DependencyRegistry.Register<LevelDataManager>();
            DependencyRegistry.Register<DeterministicRandomProvider>();
            
            DependencyRegistry.Register<CollisionController>();
            DependencyRegistry.Register<MovementController>();
            
            DependencyRegistry.Register<ObstacleDataManager>();
            DependencyRegistry.Register<PowerupDataManager>();
            DependencyRegistry.Register<ScoreManager>();
        }

        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                _levelDataManager,
                _deterministicRandomProvider,
                
                _collisionController,
                _movementController,

                _obstacleDataManager,
                _powerupDataManager,
                _scoreManager,

                new LevelStartEventCommand(),
                new WaitForGameLoopEnd(),
                new LevelEndEventCommand()
            };


            return states;
        }
    }
}