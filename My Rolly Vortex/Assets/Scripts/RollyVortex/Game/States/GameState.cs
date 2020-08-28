using System.Collections.Generic;

namespace RollyVortex
{
    public sealed class GameState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>
            {
                new LevelDataProvider(),
                new DeterministicRandomProvider(),
                new ObstacleDataProvider(),

                new ObstacleManager(),
                new PowerupManager(),
                new ScoreManager(),

                new LevelStartCommand(),
                new WaitForGameLoopEnd(),
                new LevelEndCommand()
            };


            return states;
        }
    }
}