using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS {
    public class BiasedActionChooser : RandomActionChooser {

        private float _randomChance = 0.5f;
        
        public override Action chooseAction(Action[] executableActions, WorldModel currentState) {
            if (Random.Range(0, 1) <= _randomChance) {
                return base.chooseAction(executableActions, currentState);
            }
            else {
                // Searching for the best action according to heuristics
                Action bestAction = executableActions[0];
                float bestHeuristics = float.MaxValue;
                float heuristic;

                foreach (Action action in executableActions) {
                    heuristic = action.GetHValue(currentState);
                    
                    if (heuristic < bestHeuristics) {
                        bestHeuristics = heuristic;
                        bestAction = action;
                    }
                }

                return bestAction;
            }
        }
    }
}