using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal abstract class InitializableEventCommand : IInitializable, ICommand
    {
        [Dependency] protected readonly GameEventManager _gameEventManager;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Injector.Inject(this);

            ((ICommand) this).Execute();
            onComplete?.Invoke(this);
        }

        protected abstract string GameEvent { get; }
        
        void ICommand.Execute()
        {
            if (string.IsNullOrEmpty(GameEvent)) return;

            _gameEventManager.Broadcast(GameEvent);
        }
    }
    
    internal sealed class LevelEndEventCommand : InitializableEventCommand
    {
        protected override string GameEvent => GameEvents.LevelEvents.Stop;
    }
    
    internal sealed class LevelStartEventCommand : InitializableEventCommand
    {
        protected override string GameEvent => GameEvents.LevelEvents.Start;
    }
}