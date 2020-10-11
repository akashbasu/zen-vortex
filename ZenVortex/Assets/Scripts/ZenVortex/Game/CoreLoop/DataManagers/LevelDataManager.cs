namespace ZenVortex
{
    internal class LevelDataManager : BaseDataManager<LevelData>
    {
        public LevelData CurrentLevelData => _data[_currentLevel];
        
        protected override string DataPath => GameConstants.DataPaths.Resources.Levels;
        
        private int _currentLevel;
    }
}