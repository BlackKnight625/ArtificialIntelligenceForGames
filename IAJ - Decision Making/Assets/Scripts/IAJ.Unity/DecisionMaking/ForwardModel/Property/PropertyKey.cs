using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel {
    public abstract class PropertyKey<T> : IPropertyKey {

        public int Index { get; }

        public PropertyKey() {
            Index = IPropertyKey.AMOUNT_KEYS;
            
            IPropertyKey.KEYS.Add(this);
        }

        public abstract T GetDefaultValue(GameManager gameManager);

        public object GetDefaultObjectValue(GameManager gameManager) {
            return GetDefaultValue(gameManager);
        }

        public class PropertyKeyMana : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.Mana;
            }
        }
        
        public class PropertyKeyXP : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.XP;
            }
        }
        
        public class PropertyKeyMaxHP : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.MaxHP;
            }
        }
        
        public class PropertyKeyHP : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.HP;
            }
        }
        
        public class PropertyKeyShieldHP : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.ShieldHP;
            }
        }
        
        public class PropertyKeyMoney : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.Money;
            }
        }
        
        public class PropertyKeyTime : PropertyKey<float> {
            public override float GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.Time;
            }
        }
        
        public class PropertyKeyLevel : PropertyKey<int> {
            public override int GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.baseStats.Level;
            }
        }
        
        public class PropertyKeyPosition : PropertyKey<Vector3> {
            public override Vector3 GetDefaultValue(GameManager gameManager) {
                return gameManager.Character.gameObject.transform.position;
            }
        }
    }
}