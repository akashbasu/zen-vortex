using System.IO;

namespace RollyVortex
{
    public static partial class GameConstants
    {
        public static partial class DataPaths
        {
            public static readonly string ResourcesBase = Path.Combine("Assets", "Resources");
        }
        
        public static class Procedural
        {
            public static class Tube
            {
                public const int Resolution = 72;
                public const float Height = 1f;
                private const float HeightToWidthMultiplier = 0.5f;
                private const float TubeWallWidth = 0.25f;

                public const float OuterRadius = Height * HeightToWidthMultiplier;
                public const float InnerRadius = OuterRadius * TubeWallWidth;
            }
        }
    }
}