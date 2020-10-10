namespace ZenVortex
{
    internal interface ILevelMovementObserver
    {
        void Reset();
        void Update(float deltaTime);
        void SetLevelData(LevelData data);

        void OnLevelStart();
        void OnLevelEnd();
    }
}