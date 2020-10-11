using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal class ShuffleBag
    {
        [Dependency] private readonly DeterministicRandomProvider _deterministicRandomProvider;
        
        private readonly int _size;
        private List<int> _bag;

        internal ShuffleBag(int size)
        {
            Injector.Inject(this);
            
            _size = size;
            FillBag();
        }

        public int Next()
        {
            if (_bag.Count == 0) FillBag();

            var index = _deterministicRandomProvider.Next(0, _bag.Count);
            var val = _bag[index];
            _bag.RemoveAt(index);
            return val;
        }

        private void FillBag()
        {
            _bag = new List<int>(_size);
            for (var i = 0; i < _size; i++) _bag.Add(i);
        }
    }
}