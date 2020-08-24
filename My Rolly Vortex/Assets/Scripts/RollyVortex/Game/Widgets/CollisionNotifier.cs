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
            if (string.IsNullOrEmpty(_onTriggerEnterEvent)) return;

            FireCommand(_onTriggerEnterEvent, gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(_onTriggerExitEvent)) return;

            FireCommand(_onTriggerExitEvent, gameObject, other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            if (string.IsNullOrEmpty(_onTriggerStayEvent)) return;

            FireCommand(_onTriggerStayEvent, gameObject, other.gameObject);
        }

        private static void FireCommand(string eventName, params object[] args)
        {
            var c = new Command(eventName, args);
            c.Execute();
        }
    }
}