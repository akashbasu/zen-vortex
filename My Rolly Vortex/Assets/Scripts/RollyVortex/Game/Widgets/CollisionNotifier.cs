using System;
using UnityEngine;

namespace RollyVortex
{
    public class CollisionNotifier : MonoBehaviour
    {
        [SerializeField] private string OnTriggerEnterEvent;
        [SerializeField] private string OnTriggerExitEvent;

        private void OnTriggerEnter(Collider other)
        {
            if(string.IsNullOrEmpty(OnTriggerEnterEvent)) return;
            
            GameEventManager.Broadcast(OnTriggerEnterEvent, gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if(string.IsNullOrEmpty(OnTriggerExitEvent)) return;
            
            GameEventManager.Broadcast(OnTriggerExitEvent, gameObject, other.gameObject);
        }
    }
}