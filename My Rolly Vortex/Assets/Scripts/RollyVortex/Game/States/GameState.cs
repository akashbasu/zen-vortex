using System.Collections.Generic;

namespace RollyVortex
{
    public sealed class GameState : BaseGameState
    {
        protected override List<IInitializable> GetSteps(object[] args)
        {
            var states = new List<IInitializable>();

            states.Add(new LevelManager());
            states.Add(new ObstacleManager());
            states.Add(new PowerupManager());
            states.Add(new ScoreManager());
            states.Add(new LevelStartCommand());
            states.Add(new WaitForGameLoopEnd());
            states.Add(new LevelEndCommand());

            return states;
        }
    }
}