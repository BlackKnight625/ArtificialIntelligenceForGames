using System;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Disposable : MonoBehaviour {
        public PropertyKey<bool> ExistsKey { get; private set; }

        private void Start() {
            ExistsKey = new PropertyKeyExists(gameObject.name);
        }

        private class PropertyKeyExists : PropertyKey<bool> {

            private readonly string _name;

            public PropertyKeyExists(string name) : base() {
                _name = name;
            }
            
            public override bool GetDefaultValue(GameManager gameManager) {
                return gameManager.disposableObjects.ContainsKey(_name);
            }
        }
    }
}