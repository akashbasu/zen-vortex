using System.Collections.Generic;
using UnityEngine;

namespace RollyVortex
{
    public class CollisionNotifier : MonoBehaviour
    {
        private const string Untagged = nameof(Untagged);
        
        private static Dictionary<string, GameObject> _lastFiredGameObject = new Dictionary<string, GameObject>
        {
            {GameEvents.Collisions.Start, null},
            {GameEvents.Collisions.Stay, null},
            {GameEvents.Collisions.End, null},
        };
        
        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"[{nameof(CollisionNotifier)}] {nameof(OnTriggerEnter)} {gameObject.name} {other.gameObject.name}");
            
            FireCommand(GameEvents.Collisions.Start, gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            // Debug.Log($"[{nameof(CollisionNotifier)}] {nameof(OnTriggerExit)} {gameObject.name} {other.gameObject.name}");
            
            FireCommand(GameEvents.Collisions.End, gameObject, other.gameObject);
        }

        private static void FireCommand(string eventName, GameObject go, GameObject other)
        {
            if(string.IsNullOrEmpty(eventName)) return;

            while ((string.IsNullOrEmpty(other.tag) || string.Equals(other.tag, Untagged)) && other.transform.parent != null)
            {
                other = other.transform.parent.gameObject;
            }
            
            if(string.Equals(go.tag, other.tag)) return;
            
            if(_lastFiredGameObject[eventName] == other) return;

            _lastFiredGameObject[eventName] = other;
            
            var c = new Command(eventName, go, other);
            c.Execute();
        }
    }
}