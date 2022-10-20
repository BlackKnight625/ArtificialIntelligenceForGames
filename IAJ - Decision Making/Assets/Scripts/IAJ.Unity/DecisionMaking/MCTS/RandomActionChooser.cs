using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS {
    public class RandomActionChooser : MCTSPlayoutActionChooser {
        public override Action chooseAction(Action[] executableActions, WorldModel currentState) {
            return executableActions[UnityEngine.Random.Range(0, executableActions.Length)];
        }
    }
}