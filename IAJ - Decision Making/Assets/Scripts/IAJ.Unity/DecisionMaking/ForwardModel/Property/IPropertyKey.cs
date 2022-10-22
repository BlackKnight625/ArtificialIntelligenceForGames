using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel {
    public interface IPropertyKey {
        
        public static List<IPropertyKey> KEYS { get; } = new();
        public static int AMOUNT_KEYS => KEYS.Count;
        
        object GetDefaultObjectValue(GameManager gameManager);
    }
}