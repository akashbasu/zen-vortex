namespace ZenVortex
{
    internal interface IGameLoopObserver
    {
        void Reset();
        void SetLevelData(LevelData data);
        void OnGameStart();
        void Update(float deltaTime);
        void OnGameEnd();
    }
}