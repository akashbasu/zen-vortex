using ZenVortex.DI;

namespace ZenVortex
{
    internal class ScoreUpdatedCommand : BaseUiCommand
    {
        [Dependency] private readonly IAudioServiceController _audioServiceController;
        [Dependency] private readonly IVibrationServiceController _vibrationServiceController;
        [Dependency] private readonly IScoreDataManager _scoreDataManager;
        [Dependency] private readonly IUiDataProvider _uiDataProvider;
        
        public override void Execute()
        {
            var currentScore = _scoreDataManager.CurrentScore;

            _uiDataProvider.UpdateData(UiDataKeys.Player.HighScore, _scoreDataManager.HighestScore);
            _uiDataProvider.UpdateData(UiDataKeys.Player.RunScore, currentScore.TotalScore);
                
            if(currentScore.TotalScore <= 0) return;
            
            _audioServiceController.PlayAudioForPriority(Priority.Low);
            _vibrationServiceController.PlayVibrationForPriority(Priority.Low);
        }
    }
}