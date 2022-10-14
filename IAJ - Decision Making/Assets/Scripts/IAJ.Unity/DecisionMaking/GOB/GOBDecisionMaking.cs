using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;
namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class GOBDecisionMaking
    {
        public bool InProgress { get; set; }
        private List<Goal> goals { get; set; }
        private List<Action> actions { get; set; }

        public Action secondBestAction;

        // Utility based GOB
        public GOBDecisionMaking(List<Action> _actions, List<Goal> goals)
        {
            this.actions = _actions;
            this.goals = goals;
            secondBestAction = new Action("yo");
        }


        public static float CalculateDiscontentment(Action action, List<Goal> goals)
        {
            // Keep a running total
            var discontentment = 0.0f;
            var duration = action.GetDuration();

            foreach (var goal in goals)
            {
               // Calculate the new value after the action
                var newValue = goal.InsistenceValue + action.GetGoalChange(goal);

                // The change rate is how much the goals changes per time
                newValue += duration * goal.ChangeRate;

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public Action ChooseAction()
        {
            // Find the action leading to the lowest discontentment
            InProgress = true;
            Action bestAction = actions[0];
            var bestValue = float.PositiveInfinity;
            //var bestValue = CalculateDiscontentment(actions[0], goals);
            
            foreach(Action action in actions) {
                if (action.CanExecute())
                {
                    float value = CalculateDiscontentment(action, goals);
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestAction = action;
                    }
                }
            }
            
            InProgress = false;
            return bestAction;
        }
    }
}
