using System.Collections.Generic;

namespace ZenVortex
{
    internal class UiDataEntry<TData>
    {
        private TData _data;
        private readonly HashSet<IBindable<TData>> _boundObjects;

        internal UiDataEntry(TData data)
        {
            _data = data;
            _boundObjects = new HashSet<IBindable<TData>>();
        }

        public void AddBoundObject(IBindable<TData> bindable)
        {
            _boundObjects.Add(bindable);
            bindable.UpdateData(_data);
        }

        public void RemoveBoundObject(IBindable<TData> bindable)
        {
            _boundObjects.Remove(bindable);
            bindable.UpdateData(default);
        }

        public void UpdateData(TData data)
        {
            _data = data;
            foreach (var bindable in _boundObjects) bindable.UpdateData(_data);
        }
    }

    internal interface IUiDataProvider
    {
        void UpdateData(string key, object data);
        void RegisterLabel(string key, IBindable<string> textBinder);
        void UnRegisterLabel(string key, IBindable<string> textBinder);
    }
    
    internal class UiDataProvider : IUiDataProvider
    {
        private readonly Dictionary<string, UiDataEntry<string>> _directory = new Dictionary<string, UiDataEntry<string>>();
        
        public void UpdateData(string key, object data)
        {
            if (_directory.ContainsKey(key))
            {
                _directory[key].UpdateData(data.ToString());
            }
            else
            {
                _directory[key] = new UiDataEntry<string>(data.ToString());
            }
        }

        public void RegisterLabel(string key, IBindable<string> textBinder)
        {
            if (_directory.ContainsKey(key))
            {
                _directory[key].RemoveBoundObject(textBinder);
            }
            else
            {
                _directory[key] = new UiDataEntry<string>(default);
            }
            
            _directory[key].AddBoundObject(textBinder);
        }
        
        public void UnRegisterLabel(string key, IBindable<string> textBinder)
        {
            if(!_directory.ContainsKey(key)) return;
            
            _directory[key].RemoveBoundObject(textBinder);
        }
    }
}