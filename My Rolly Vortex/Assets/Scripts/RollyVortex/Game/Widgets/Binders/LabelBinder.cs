using TMPro;
using UnityEngine;

namespace RollyVortex
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal class LabelBinder : MonoBehaviour, IBindable<string>
    {
        [SerializeField] private string _uiKey;
        
        private TextMeshProUGUI _label;
        
        private void Awake()
        {
            _label = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_uiKey)) return;

            UiDataProvider.RegisterLabel(_uiKey, this);
        }

        private void OnDisable()
        {
            UiDataProvider.UnRegisterLabel(_uiKey, this);
        }

        public void UpdateData(string data)
        {
            _label.text = data;
        }
    }
}