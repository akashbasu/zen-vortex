namespace ZenVortex
{
    internal interface ILevelDataManager
    {
        LevelData CurrentLevelData { get; }
    }
    
    internal class LevelDataManager : BaseResourceDataManager<LevelData>, ILevelDataManager
    {
        public LevelData CurrentLevelData => _data[_currentLevel];
        
        protected override string DataPath => GameConstants.DataPaths.Resources.Levels;
        
        private int _currentLevel = 0;
    }
}