using System;
using UnityEngine;

namespace RollyVortex
{
    [Serializable]
    public struct RangedValue
    {
        [SerializeField] private int _min;
        [SerializeField] private int _max;

        public int Min => _min;
        public int Max => _max;
    }
}