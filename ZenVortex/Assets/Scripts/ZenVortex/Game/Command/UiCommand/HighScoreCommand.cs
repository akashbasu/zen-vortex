using ZenVortex.DI;

namespace ZenVortex
{
    internal class HighScoreCommand : BaseUiCommand
    {
        [Dependency] private readonly IAudioServiceController _audioServiceController;
        [Dependency] private readonly IVibrationServiceController _vibrationServiceController;
        
        public override void Execute()
        {
            _audioServiceController.PlayAudioForPriority(Priority.Medium);
            _vibrationServiceController.PlayVibrationForPriority(Priority.Medium);
        }
    }
}