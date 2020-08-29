using UnityEngine;

namespace RollyVortex
{
    public interface ILevelMovement
    {
        void Reset();
        void Update(float deltaTime);
        void SetLevelData(LevelData data);

        void OnCollisionEnter(GameObject other);
        void OnCollisionStay(GameObject other);
        void OnCollisionExit(GameObject other);

        void OnLevelStart();
        void OnLevelEnd();
    }
}