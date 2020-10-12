using System;
using ZenVortex.DI;

namespace ZenVortex
{
    internal abstract class InitializableEventCommand : IInitializable, ICommand
    {
        [Dependency] protected readonly IGameEventManager _gameEventManager;
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Injector.ResolveDependencies(this);

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
}