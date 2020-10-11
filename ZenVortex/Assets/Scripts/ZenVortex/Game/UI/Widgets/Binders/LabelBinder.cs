using TMPro;
using UnityEngine;
using ZenVortex.DI;

namespace ZenVortex
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal class LabelBinder : MonoBehaviour, IBindable<string>
    {
        [SerializeField] private string _uiKey;
        [SerializeField] private string _format;
        [SerializeField] private bool _animateOnChange = true;

        [Dependency] private readonly IUiDataProvider _uiDataProvider;
        
        private TextMeshProUGUI _label;
        
        private void Awake()
        {
            Injector.ResolveDependencies(this);
            
            _label = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (!string.IsNullOrWhiteSpace(_format)) _label.text = string.Empty;
            
            if(string.IsNullOrEmpty(_uiKey)) return;

            _uiDataProvider.RegisterLabel(_uiKey, this);
        }

        private void OnDisable()
        {
            _uiDataProvider.UnRegisterLabel(_uiKey, this);
        }

        public void UpdateData(string data)
        {
            if(_label.text == data) return;
            
            _label.text = !string.IsNullOrWhiteSpace(_format) ? string.Format(_format, data) : data;
            if (_animateOnChange)
            {
                // LeanTween.
            }
        }
    }
}