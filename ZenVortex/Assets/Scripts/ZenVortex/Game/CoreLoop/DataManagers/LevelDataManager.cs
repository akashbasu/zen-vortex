namespace ZenVortex
{
    internal class LevelDataManager : BaseResourceDataManager<LevelData>
    {
        public LevelData CurrentLevelData => _data[_currentLevel];
        
        protected override string DataPath => GameConstants.DataPaths.Resources.Levels;
        
        private int _currentLevel;
    }
}