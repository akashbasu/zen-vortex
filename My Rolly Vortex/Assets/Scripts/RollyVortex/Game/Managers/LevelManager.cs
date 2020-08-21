using System;

namespace RollyVortex
{
    public class LevelManager : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if(TryLoadLevelData())
            {
                GameEventManager.Broadcast(GameEvents.LevelEvents.StartLevel, 1f);
                //onComplete?.Invoke(this);
            }
        }

        private bool TryLoadLevelData()
        {
            return true;
        }
    }
}