using ZenVortex.DI;

namespace ZenVortex
{
    internal class BallScaleUpdatedCommand : BaseUiCommand
    {
        [Dependency] private readonly BallMovement _ballMovement;
        [Dependency] private readonly IUiDataProvider _uiDataProvider;
        
        public override void Execute()
        {
            _uiDataProvider.UpdateData(UiDataKeys.Ball.Scale, _ballMovement.CurrentScale.ToString("N2"));
        }
    }
}