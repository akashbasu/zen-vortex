using UnityEngine;

namespace RollyVortex
{
    public class CollisionNotifier : MonoBehaviour
    {
        [SerializeField] private string _onTriggerEnterEvent;
        [SerializeField] private string _onTriggerExitEvent;
        [SerializeField] private string _onTriggerStayEvent;

        private void OnTriggerEnter(Collider other)
        {
            FireCommand(_onTriggerEnterEvent, gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            FireCommand(_onTriggerExitEvent, gameObject, other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            FireCommand(_onTriggerStayEvent, gameObject, other.gameObject);
        }

        private static void FireCommand(string eventName, GameObject go, GameObject other)
        {
            if(string.IsNullOrEmpty(eventName)) return;
            
            if(string.Equals(go.tag, other.tag)) return;
            
            var c = new Command(eventName, go, other);
            c.Execute();
        }
    }
}