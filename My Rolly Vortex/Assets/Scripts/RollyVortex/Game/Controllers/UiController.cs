using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RollyVortex
{
    public class UiController : IInitializable
    {
        private GameObject _root;

        private Dictionary<string, List<GameObject>> _gameStateToUiMap;
        
        
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            if (GetReferences())
            {
                GameEventManager.Subscribe(GameEvents.GameStateEvents.Start, OnGameStateStart);
                GameEventManager.Subscribe(GameEvents.GameStateEvents.End, OnGameStateEnd);
                onComplete?.Invoke(this);    
            }
            
            Debug.LogError($"[{nameof(UiController)}] {nameof(Initialize)} Failed to find references!");
        }

        private bool GetReferences()
        {
            if (!DirectoryManager.TryGetEntry(Tags.UiRoot, out var uiRoot)) return false;

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
                Debug.LogError($"[{nameof(UiController)}] {nameof(OnGameStateStart)} No UI state found for {currentState}");
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