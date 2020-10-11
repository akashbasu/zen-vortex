using ZenVortex.DI;

namespace ZenVortex
{
    internal class EventCommand : ICommand
    {
        [Dependency] private readonly GameEventManager _gameEventManager;
        
        private readonly object[] _args;
        private readonly string _event;

        internal EventCommand(string eventName, params object[] args)
        {
            Injector.ResolveDependencies(this);
            
            _event = eventName;
            _args = args;
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(_event)) return;

            _gameEventManager.Broadcast(_event, _args);
        }
    }
}