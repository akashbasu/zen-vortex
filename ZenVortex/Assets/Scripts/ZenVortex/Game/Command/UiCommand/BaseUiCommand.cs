using ZenVortex.DI;

namespace ZenVortex
{
    internal abstract class BaseUiCommand : ICommand
    {
        protected BaseUiCommand()
        {
            Injector.ResolveDependencies(this);
        }

        public abstract void Execute();
    }
}