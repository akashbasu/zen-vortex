using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    internal interface IUiServiceController : IPostConstructable {}
    
    internal class UiServiceController : IUiServiceController
    {
        [Dependency] private readonly IGameEventManager _gameEventManager;
        [Dependency] private readonly ISceneReferenceProvider _sceneReferenceProvider;
        
        private GameObject _root;

        private Dictionary<string, List<GameObject>> _gameStateToUiMap;
        
        public void PostConstruct(params object[] args)
        {
            if (GetReferences())
            {
                _gameEventManager.Subscribe(GameEvents.StateMachineEvents.Start, OnGameStateStart);
                _gameEventManager.Subscribe(GameEvents.StateMachineEvents.End, OnGameStateEnd);
                return;
            }
            
            Debug.LogError($"[{nameof(UiServiceController)}] {nameof(PostConstruct)} Failed to find references!");
        }

        public void Dispose()
        {
            _gameEventManager.Unsubscribe(GameEvents.StateMachineEvents.Start, OnGameStateStart);
            _gameEventManager.Unsubscribe(GameEvents.StateMachineEvents.End, OnGameStateEnd);
        }

        private bool GetReferences()
        {
            if (!_sceneReferenceProvider.TryGetEntry(Tags.UiRoot, out var uiRoot)) return false;

            _root = uiRoot;
            _gameStateToUiMap = new Dictionary<string, List<GameObject>>(_root.transform.childCount);

            foreach (Transform child in _root.transform)
            {
                var objectsForState = new List<GameObject>(child.childCount);
                objectsForState.AddRange(from Transform stateObjects in child select stateObjects.gameObject);
                _gameStateToUiMap.Add(child.name, objectsForState);
            }

            return _gameStateToUiMap.Count > 0;
        }
        
        private void OnGameStateEnd(object[] obj)
        {
            DisableAllObjects();
        }

        private void OnGameStateStart(object[] obj)
        {
            if (obj?.Length < 1) return;
            
            var currentState = (string) obj[0]; 
            if (_gameStateToUiMap.ContainsKey(currentState))
            {
                DisableAllObjects();
                EnableCurrent(currentState);
            }
            else
            {
                DisableAllObjects();
                Debug.LogError($"[{nameof(UiServiceController)}] {nameof(OnGameStateStart)} No UI state found for {currentState}");
            }
        }
        
        private void EnableCurrent(string currentState)
        {
            foreach (var stateObject in _gameStateToUiMap[currentState])
            {
                stateObject.SetActive(true);
            }
        }
        
        private void DisableAllObjects()
        {
            foreach (var gameObject in _gameStateToUiMap.Values.SelectMany(gameObjectsForState => gameObjectsForState))
            {
                gameObject.SetActive(false);
            }
        }
    }
}