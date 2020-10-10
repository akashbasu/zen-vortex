using UnityEngine;

namespace ZenVortex
{
    internal interface ICollisionEventObserver
    {
        void OnCollisionEnter(GameObject other, int pointOfCollision);
        void OnCollisionStay(GameObject other);
        void OnCollisionExit(GameObject other, int pointOfCollision);
    }
}