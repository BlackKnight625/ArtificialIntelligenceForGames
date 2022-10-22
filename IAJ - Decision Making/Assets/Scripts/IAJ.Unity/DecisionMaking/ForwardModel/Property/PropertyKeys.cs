using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel {
    public class PropertyKeys {
        public static PropertyKey<int> MANA { get; } = new PropertyKey<int>.PropertyKeyMana();
        public static PropertyKey<int> XP { get; } = new PropertyKey<int>.PropertyKeyXP();
        public static PropertyKey<int> MAXHP { get; } = new PropertyKey<int>.PropertyKeyMaxHP();
        public static PropertyKey<int> HP { get; } = new PropertyKey<int>.PropertyKeyHP();
        public static PropertyKey<int> ShieldHP { get; } = new PropertyKey<int>.PropertyKeyShieldHP();
        public static PropertyKey<int> MONEY { get; } = new PropertyKey<int>.PropertyKeyMoney();
        public static PropertyKey<float> TIME { get; } = new PropertyKey<float>.PropertyKeyTime();
        public static PropertyKey<int> LEVEL { get; } = new PropertyKey<int>.PropertyKeyLevel();
        public static PropertyKey<Vector3> POSITION { get; } = new PropertyKey<Vector3>.PropertyKeyPosition();

        public static void Load() {
            // Do nothing
        }
    }
}