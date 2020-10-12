using ZenVortex.DI;

namespace ZenVortex
{
    internal class LivesUpdatedCommand : BaseUiCommand
    {
        [Dependency] private readonly IPlayerLifeDataManager _playerLifeDataManager;
        [Dependency] private readonly IUiDataProvider _uiDataProvider;
        
        public override void Execute()
        {
            _uiDataProvider.UpdateData(UiDataKeys.Player.Lives, _playerLifeDataManager.LifeCount);
        }
    }
}