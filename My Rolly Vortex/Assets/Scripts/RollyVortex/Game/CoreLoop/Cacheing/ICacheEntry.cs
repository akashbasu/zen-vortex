using System;
using UnityEngine;

namespace RollyVortex
{
    internal interface ICacheEntry
    {
        Transform Transform { get; }
        bool HasActionableCollision { get; }
        
        void Reset();
        void Spawn(params object[] args);
        void Fire(float distanceToTravel, float time, Action onComplete, params object[] args);
        void Pause();
        
        void CollisionStart(params object[] args);
        void CollisionComplete(params object[] args);
    }
}