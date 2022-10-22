using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {

        public PickUpChest(AutonomousCharacter character, Disposable target) : base("PickUpChest",character,target)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute()
        {

            if (!base.CanExecute())
                return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }

        public override void Execute()
        {
            
            base.Execute();
            GameManager.Instance.PickUpChest(this.Target.gameObject);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);

            var money = worldModel.GetProperty(PropertyKeys.MONEY);
            worldModel.SetProperty(PropertyKeys.MONEY, money + 5);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            return -10 + GetDuration(worldModel) / 2f;
        }
    }
}
