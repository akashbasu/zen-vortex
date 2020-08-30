using UnityEngine;

namespace RollyVortex
{
    public interface ILevelMovement
    {
        void Reset();
        void Update(float deltaTime);
        void SetLevelData(LevelData data);

        void OnCollisionEnter(GameObject other, int pointOfCollision);
        void OnCollisionStay(GameObject other);
        void OnCollisionExit(GameObject other, int pointOfCollision);

        void OnLevelStart();
        void OnLevelEnd();
    }
}