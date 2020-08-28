using UnityEngine;

namespace RollyVortex
{
    public static class MovementUtils
    {
        public static void SetTexturePosition(Material material, int textureId, float x, float y)
        {
            material.SetTextureOffset(textureId, new Vector2(x, y));
        }

        public static void UpdateTexturePositionY(ref float lastTime, ref float currentYOffset, float tiling,
            float deltaTime, float totalTime, Material material, int textureId)
        {
            lastTime += deltaTime;
            lastTime %= totalTime;
            currentYOffset = Mathf.Lerp(0, tiling, lastTime / totalTime);
            currentYOffset %= tiling;
            SetTexturePosition(material, textureId, material.GetTextureOffset(textureId).x, -currentYOffset);
        }

        public static void SetBallRotation(Transform anchor, float z)
        {
            anchor.rotation = Quaternion.Euler(0, 0, z);
        }

        public static void UpdateBallPosition(ref float lastTime, Transform anchor, float deltaTime,
            float targetRotation, float totalTime)
        {
            lastTime += deltaTime;
            var currentRotation = anchor.rotation;
            var wantedRotation = Quaternion.Euler(0, 0, targetRotation);
            anchor.rotation = Quaternion.Lerp(currentRotation, wantedRotation, lastTime / totalTime);
        }

        public static void SetPositionForObstacle(Transform obstacle, float z)
        {
            var currentPos = obstacle.localPosition;
            currentPos.z = z;
            obstacle.localPosition = currentPos;
        }
    }
}