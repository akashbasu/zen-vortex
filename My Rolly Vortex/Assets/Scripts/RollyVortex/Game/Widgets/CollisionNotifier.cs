using UnityEngine;

namespace RollyVortex
{
    public class CollisionNotifier : MonoBehaviour
    {
        private const string Untagged = nameof(Untagged);

        private void OnTriggerEnter(Collider other)
        {
            FireCommand(GameEvents.Collisions.Start, gameObject, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            FireCommand(GameEvents.Collisions.End, gameObject, other.gameObject);
        }

        private static void FireCommand(string eventName, GameObject go, GameObject other)
        {
            if (string.IsNullOrEmpty(eventName)) return;

            var siblingIndex = other.transform.GetSiblingIndex();
            while ((string.IsNullOrEmpty(other.tag) || string.Equals(other.tag, Untagged)) &&
                   other.transform.parent != null) other = other.transform.parent.gameObject;

            if (string.Equals(go.tag, other.tag)) return;

            var c = new Command(eventName, go, other, siblingIndex);
            c.Execute();
        }
    }
}