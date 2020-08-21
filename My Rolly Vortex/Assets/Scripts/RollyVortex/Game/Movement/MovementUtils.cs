using UnityEngine;

namespace RollyVortex
{
    public static class MovementUtils
    {
        public static void SetTexturePosition(Material material, int textureId, float x, float y)
        {
            material.SetTextureOffset(textureId, new Vector2(x, y));
        }

        public static void UpdateTexturePositionY(ref float lastTime, ref float currentOffset, float tiling, float deltaTime, float speed, Material material, int textureId)
        {
            lastTime += deltaTime;
            lastTime %= speed;
            currentOffset = Mathf.Lerp(0, tiling, lastTime / speed);
            currentOffset %= tiling;
            SetTexturePosition(material, textureId, material.GetTextureOffset(textureId).x, -currentOffset);
        }
    }
}