using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal abstract class InitializableCommand : IInitializable
    {
        [Dependency] protected readonly GameEventManager _gameEventManager;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Injector.Inject(this);
            
            Execute(args);
            onComplete?.Invoke(this);
        }

        protected abstract string GameEvent { get; }
        
        private void Execute(object[] args)
        {
            if (string.IsNullOrEmpty(GameEvent)) return;

            _gameEventManager.Broadcast(GameEvent, args);
        }
    }
    
    internal sealed class LevelEndCommand : InitializableCommand
    {
        protected override string GameEvent => GameEvents.LevelEvents.Stop;
    }
    
    internal sealed class LevelStartCommand : InitializableCommand
    {
        protected override string GameEvent => GameEvents.LevelEvents.Start;
    }
}