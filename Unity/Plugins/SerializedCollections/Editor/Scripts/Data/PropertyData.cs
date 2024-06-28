using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor.Data
{
    [System.Serializable]
    internal class PropertyData
    {
        
        private float _keyLabelWidth;
        
        private ElementData _keyData;
        
        private ElementData _valueData;
        
        private bool _alwaysShowSearch = false;

        public bool AlwaysShowSearch
        {
            get => _alwaysShowSearch;
            set => _alwaysShowSearch = value;
        }

        public float KeyLabelWidth
        {
            get => _keyLabelWidth;
            set => _keyLabelWidth = value;
        }

        public ElementData GetElementData(bool fieldType)
        {
            return fieldType == SCEditorUtility.KeyFlag ? _keyData : _valueData;
        }

        public PropertyData() : this(new ElementSettings(), new ElementSettings()) { }

        public PropertyData(ElementSettings keySettings, ElementSettings valueSettings)
        {
            _keyData = new ElementData(keySettings);
            _valueData = new ElementData(valueSettings);
        }
    }
}