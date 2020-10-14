using ZenVortex.DI;

namespace ZenVortex
{
    internal class TimeScaleUpdatedCommand : BaseUiCommand
    {
        [Dependency] private readonly ITimeServiceController _timeServiceController;
        [Dependency] private readonly IUiDataProvider _uiDataProvider;
        
        public override void Execute()
        {
            _uiDataProvider.UpdateData(UiDataKeys.Time.Scale, _timeServiceController.CurrentTimeScale.ToString("N2"));
        }
    }
}