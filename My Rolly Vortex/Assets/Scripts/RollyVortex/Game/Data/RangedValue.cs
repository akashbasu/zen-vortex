using System;
using UnityEngine;

namespace RollyVortex
{
    [Serializable]
    public class RangedValue<T>
    {
        [SerializeField] private T _min;
        [SerializeField] private T _max;

        public T Min => _min;
        public T Max => _max;

        public RangedValue(T min, T max)
        {
            _min = min;
            _max = max;
        }

        public static bool IsInitialized(RangedValue<T> obj)
        {
            return obj != null && !(obj._min.Equals(default(T)) && obj._max.Equals(default(T)));
        }
    }

    [Serializable]
    public class IntRangedValue : RangedValue<int>
    {
        public IntRangedValue(int min, int max) : base(min, max){}
    }
}