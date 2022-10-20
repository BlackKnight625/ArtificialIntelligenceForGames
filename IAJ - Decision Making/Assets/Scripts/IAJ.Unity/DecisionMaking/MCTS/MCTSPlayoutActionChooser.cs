using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS {
    public abstract class MCTSPlayoutActionChooser {
        public abstract Action chooseAction(Action[] executableActions, WorldModel currentState);
    }
}