using System;
using UnityEngine;

namespace ZenVortex
{
    [Serializable]
    public class RangedValue<T>
    {
        [SerializeField] protected T _min;
        [SerializeField] protected T _max;
        
        public T Min => _min;
        public T Max => _max;

        internal RangedValue(T min, T max)
        {
            _min = min;
            _max = max;
        }

        public static bool IsInitialized(RangedValue<T> obj)
        {
            return obj != null && !(obj._min.Equals(default(T)) && obj._max.Equals(default(T)));
        }

        public override string ToString()
        {
            return $"Min : {_min} Max : {_max}";
        }
    }

    [Serializable]
    public class IntRangedValue : RangedValue<int>
    {
        [SerializeField] private bool _isZeroBased;

        public IntRangedValue(int min, int max, bool isZeroBased = false) : base(min, max)
        {
            _isZeroBased = isZeroBased;
        }

        public int Interval => !_isZeroBased ? _max - _min : (_min < default(int) || _max < default(int)) ? default : _max - _min + 1 ;

        public override string ToString()
        {
            return $"{base.ToString()} Interval {Interval}";
        }

        public bool IsEqualTo(IntRangedValue other)
        {
            return other != null && other.Min == _min && other._max == _max && other.Interval == Interval;
        }
    }
}