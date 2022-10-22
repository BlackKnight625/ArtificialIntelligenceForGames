using System;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel {
    public class PropertyStorage {

        private readonly object[] _properties;
        
        public PropertyStorage() {
            // Creating a brand new empty properties array
            _properties = new object[IPropertyKey.AMOUNT_KEYS];

            ResetToDefaults(GameManager.Instance);
        }

        private PropertyStorage(PropertyStorage old) {
            int length = old._properties.Length;
            
            _properties = new object[old._properties.Length];

            for (int i = 0; i < length; i++) {
                _properties[i] = old._properties[i];
            }
        }
        
        public T GetProperty<T>(PropertyKey<T> key) {
            return (T) _properties[key.Index];
        }

        public void SetProperty<T>(PropertyKey<T> key, T newValue) {
            _properties[key.Index] = newValue;
        }

        public PropertyStorage GetChildStorage() {
            return new PropertyStorage(this);
        }

        public void ResetToDefaults(GameManager gameManager) {
            // Filling up the properties with their default values
            List<IPropertyKey> keys = IPropertyKey.KEYS;

            for (int i = 0; i < keys.Count; i++) {
                _properties[i] = keys[i].GetDefaultObjectValue(gameManager);
            }
        }
    }
}