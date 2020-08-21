using UnityEngine;

namespace RollyVortex
{
    public interface ILevelMovement
    {
        bool IsEnabled { get; set; }
        void Reset();
        void Update(float deltaTime);
        void SetLevelData(float speed);

        void OnCollisionEnter(GameObject other);
        void OnCollisionExit(GameObject other);

        void OnLevelStart();
        void OnLevelEnd();
    }
}