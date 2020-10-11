using UnityEngine;
using UnityEngine.UI;
using ZenVortex.DI;

namespace ZenVortex
{
    [RequireComponent(typeof(Button))]
    internal class ButtonWidget : MonoBehaviour
    {
        [SerializeField] private string _onClickEvent;

        [Dependency] private readonly GameEventManager _gameEventManager;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            Injector.Inject(this);
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
            
            _gameEventManager.Broadcast(_onClickEvent);
        }
    }
}