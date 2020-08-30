using System;
using UnityEngine;

namespace RollyVortex
{
    public class PowerupController : ICacheEntry
    {
        public Transform Transform { get; }
        public bool HasActionableCollision { get; }
        
        public void Reset()
        {
            
        }

        public void Spawn(params object[] args)
        {
            
        }

        public void Fire(float distanceToTravel, float time, Action onComplete, params object[] args)
        {
            
        }

        public void CollisionStart(params object[] args)
        {
            
        }

        public void CollisionComplete(params object[] args)
        {
            
        }
    }
}