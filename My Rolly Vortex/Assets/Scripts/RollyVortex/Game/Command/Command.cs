using System;

namespace RollyVortex
{
    public class Command
    {
        private string _event;
        private object[] _args;

        public Command(string eventName, params object[] args)
        {
            _event = eventName;
            _args = args;
        }
        
        public void Execute()
        {
            if (string.IsNullOrEmpty(_event)) return;
            
            GameEventManager.Broadcast(_event, _args);
        }
        
        protected void Execute(params object[] args)
        {
            if (string.IsNullOrEmpty(_event)) return;
            
            GameEventManager.Broadcast(_event, args);
        }
    }
    
    public abstract class InitializableCommand : Command, IInitializable
    {
        protected InitializableCommand(string eventName, params object[] args) : base(eventName, args) { }

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Execute(args);
            onComplete?.Invoke(this);
        }
    }
}