using UnityEngine;
using UnityEngine.UI;

namespace ZenVortex
{
    [RequireComponent(typeof(Button))]
    internal class ButtonWidget : MonoBehaviour
    {
        [SerializeField] private string _onClickEvent;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if(string.IsNullOrEmpty(_onClickEvent)) return;
            
            GameEventManager.Broadcast(_onClickEvent);
        }
    }
}