using System;

namespace RollyVortex
{
    internal class Command
    {
        private readonly object[] _args;
        private readonly string _event;

        internal Command(string eventName, params object[] args)
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

    internal abstract class InitializableCommand : Command, IInitializable
    {
        protected InitializableCommand(string eventName, params object[] args) : base(eventName, args)
        {
        }

        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            Execute(args);
            onComplete?.Invoke(this);
        }
    }
}