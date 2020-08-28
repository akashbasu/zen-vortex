using System.IO;

namespace RollyVortex
{
    public static class GameConstants
    {
        public static class EnvironmentConstants
        {
            public const float MasterToTubeSpeedRatio = 1f;
            public const float MasterToObstacleSpeedRatio = 2f;
            public const float MasterToBallTilingRatio = 1f;
        }

        public static class DataPaths
        {
            public static readonly string LevelDataPath = Path.Combine("Data", "Levels");
            public static readonly string ObstacleDataPath = Path.Combine("Data", "Obstacles");
        }
    }
}