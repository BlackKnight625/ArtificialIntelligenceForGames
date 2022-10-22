using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System.Collections.Generic;

namespace Assets.Scripts.Game
{
    //class that represents a world model that corresponds to the current state of the world,
    //all required properties and goals are stored inside the game manager
    public class CurrentStateWorldModel : FutureStateWorldModel
    {
        private Dictionary<string, Goal> Goals { get; set; }

        public CurrentStateWorldModel(GameManager gameManager, List<Action> actions, List<Goal> goals) : base(gameManager, actions.ToArray())
        {
            this.Parent = null;
            this.Goals = new Dictionary<string, Goal>();

            foreach (var goal in goals)
            {
                this.Goals.Add(goal.Name,goal);
            }
        }

        public void Initialize()
        {
            PropertyStorage.ResetToDefaults(GameManager);
            ResetExecutableActions();
        }

        public override float GetGoalValue(string goalName) {
            return this.Goals[goalName].InsistenceValue;
        }

        public override void SetGoalValue(string goalName, float goalValue) {
            //this method does nothing, because you should not directly set a goal value of the CurrentStateWorldModel
        }

        public override int GetNextPlayer()
        {
            //in the current state, the next player is always player 0
            return 0;
        }
    }
}
