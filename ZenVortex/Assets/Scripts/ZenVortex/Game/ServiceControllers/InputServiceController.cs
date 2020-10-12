using System.Collections.Generic;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IInputServiceController : IPostConstructable
    {
        GameInputAdapter GameInput { get;}
    }
    
    internal class InputServiceController : IInputServiceController
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        
        private UnityInput _input;
        
        private readonly List<string> _gameStates = new List<string>
        {
            nameof(GameState)
        };

        public GameInputAdapter GameInput { get; private set; }
        private UiInputAdapter UiInput { get; set; }

        public void PostConstruct(params object[] args)
        {
            _input = new UnityInput();

            GameInput = new GameInputAdapter(_input.Game);
            UiInput = new UiInputAdapter(_input.UI);

            UiInput.SetEnabled(false);
            GameInput.SetEnabled(false);

            _gameEventManager.Subscribe(GameEvents.StateMachineEvents.Start, OnGameStateStart);
            _gameEventManager.Subscribe(GameEvents.StateMachineEvents.End, OnGameStateEnd);
        }
        
        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.StateMachineEvents.Start, OnGameStateStart);
            _gameEventManager.Unsubscribe(GameEvents.StateMachineEvents.End, OnGameStateEnd);
        }

        private void OnGameStateStart(object[] obj)
        {
            if (obj?.Length < 1) return;
            
            var currentState = (string) obj[0];

            var isGameState = _gameStates.Contains(currentState);
            GameInput.SetEnabled(isGameState);
            UiInput.SetEnabled(!isGameState);
        }
        
        private void OnGameStateEnd(object[] obj)
        {
            GameInput.SetEnabled(false);
            UiInput.SetEnabled(false);
        }
    }
}