using System;
using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.HP < Character.baseStats.MaxHP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return worldModel.GetProperty(PropertyKeys.HP) < worldModel.GetProperty(PropertyKeys.MAXHP);
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.GetHealthPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change -= goal.InsistenceValue;
            }

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            int health = worldModel.GetProperty(PropertyKeys.HP);
            
            int gainedHealth = this.GetGainedHeath(worldModel);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL,
                worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - gainedHealth);
            worldModel.SetProperty(PropertyKeys.HP, health + gainedHealth);

                //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target, false);
        }

        public int GetGainedHeath(WorldModel worldModel)
        {
            return worldModel.GetProperty(PropertyKeys.MAXHP) - worldModel.GetProperty(PropertyKeys.HP);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            return -this.GetGainedHeath(worldModel) + GetDuration(worldModel) / 2f;
        }
    }
}
