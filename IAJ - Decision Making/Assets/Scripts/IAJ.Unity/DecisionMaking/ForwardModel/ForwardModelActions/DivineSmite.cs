using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int enemyAC;
        private int enemySimpleDamage;
        //how do you like lambda's in c#?
        private Func<int> dmgRoll;

        private bool isSkeleton = false;

        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.dmgRoll = () => RandomHelper.RollD6();
                this.enemySimpleDamage = 0;
                this.expectedHPChange = 0;
                this.xpChange = 3;
                this.expectedXPChange = 2.7f;
                this.enemyAC = 10;
                isSkeleton = true;
            }
            else
            {
                this.dmgRoll = () => 0;
                this.enemySimpleDamage = 0;
                this.expectedHPChange = 0;
                this.xpChange = 0;
                this.expectedXPChange = 0;
                this.enemyAC = 0;
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
            GameManager.Instance.DivineSmite(this.Target);
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
            if (!base.CanExecute() || !isSkeleton || (int) worldModel.GetProperty(Properties.MANA) < 2)
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

            int mana = (int)worldModel.GetProperty(Properties.MANA);
            int xp = (int)worldModel.GetProperty(Properties.XP);

            worldModel.SetProperty(Properties.MANA, mana - 2); 
            worldModel.SetProperty(this.Target.name, false);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            
            var gainLevelValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, gainLevelValue - xpChange);
        }

        public override float GetHValue(WorldModel worldModel) {
            return 2 - xpChange * 2 + GetDuration(worldModel) / 2f; // More or less arbitrary value
        }
    }
}