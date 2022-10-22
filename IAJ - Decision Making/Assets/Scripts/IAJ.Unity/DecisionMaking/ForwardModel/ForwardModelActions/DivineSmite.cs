using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private float expectedXPChange;
        private int xpChange;

        private bool isSkeleton = false;

        public DivineSmite(AutonomousCharacter character, Disposable target) : base("DivineSmite",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.xpChange = 3;
                this.expectedXPChange = 2.7f;
                isSkeleton = true;
            }
            else
            {
                this.xpChange = 0;
                this.expectedXPChange = 0;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.expectedXPChange;
            }
            
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.DivineSmite(this.Target.gameObject);
        }
        
        public override bool CanExecute()
        {
            if (!base.CanExecute() || !isSkeleton || Character.baseStats.Mana < 2)
            //|| (Character.GetDistanceToTarget(Character.transform.position,
            //            Target.transform.position) > 4))
            {
                return false;
            }
            return true;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute() || !isSkeleton || worldModel.GetProperty(PropertyKeys.MANA) < 2)
            //    || (Character.GetDistanceToTarget((Vector3) worldModel.GetProperty(Properties.POSITION), 
            //        Target.transform.position) > 4))
            {
                return false;
            }
            return true;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int mana = worldModel.GetProperty(PropertyKeys.MANA);
            int xp = worldModel.GetProperty(PropertyKeys.XP);

            worldModel.SetProperty(PropertyKeys.MANA, mana - 2); 
            worldModel.SetProperty(this.Target, false);
            worldModel.SetProperty(PropertyKeys.XP, xp + this.xpChange);
            
            var gainLevelValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, gainLevelValue - xpChange);
        }

        public override float GetHValue(WorldModel worldModel) {
            return 2 - xpChange * 2 + GetDuration(worldModel) / 2f; // More or less arbitrary value
        }
    }
}