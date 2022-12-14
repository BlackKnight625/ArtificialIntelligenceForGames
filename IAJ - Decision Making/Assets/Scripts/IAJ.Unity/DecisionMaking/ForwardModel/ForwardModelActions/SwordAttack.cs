using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class SwordAttack : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int enemyAC;
        private int enemySimpleDamage;
        //how do you like lambda's in c#?
        private Func<int> dmgRoll;

        public SwordAttack(AutonomousCharacter character, Disposable target) : base("SwordAttack",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.dmgRoll = () => RandomHelper.RollD6();
                this.enemySimpleDamage = 3;
                this.expectedHPChange = 3.5f;
                this.xpChange = 3;
                this.expectedXPChange = 2.7f;
                this.enemyAC = 10;
            }
            else if (target.tag.Equals("Orc"))
            {
                this.dmgRoll = () => RandomHelper.RollD10() + 2;
                this.enemySimpleDamage = 8;
                this.expectedHPChange = 7.5f;
                this.xpChange = 10;
                this.expectedXPChange = 7.0f;
                this.enemyAC = 14;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.dmgRoll = () => RandomHelper.RollD12() + RandomHelper.RollD12();
                this.enemySimpleDamage = 15;
                this.expectedHPChange = 13.0f;
                this.xpChange = 20;
                this.expectedXPChange = 10.0f;
                this.enemyAC = 18;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += this.expectedHPChange;
            }
            else if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.expectedXPChange;
            }
            
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            //if (Character.GetDistanceToTarget(Character.transform.position,
            //        Target.transform.position) > 4) return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            //if (Character.GetDistanceToTarget((Vector3)worldModel.GetProperty(Properties.POSITION),
            //        Target.transform.position) > 4) return false;
            return true;
        }
        
        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.SwordAttack(this.Target.gameObject);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int hp = worldModel.GetProperty(PropertyKeys.HP);
            int shieldHp = worldModel.GetProperty(PropertyKeys.ShieldHP);
            int xp = worldModel.GetProperty(PropertyKeys.XP);

            int damage = 0;
            if (GameManager.Instance.StochasticWorld)
            {
                //execute the lambda function to calculate received damage based on the creature type
                damage = this.dmgRoll.Invoke();
            }
            else
            {
                damage = this.enemySimpleDamage;
            }
            //calculate player's damage
            int remainingDamage = damage - shieldHp;
            int remainingShield = Mathf.Max(0, shieldHp - damage);
            int remainingHP;

            if(remainingDamage > 0)
            {
                remainingHP = (hp - remainingDamage);
                worldModel.SetProperty(PropertyKeys.HP, remainingHP);
            }

            worldModel.SetProperty(PropertyKeys.ShieldHP, remainingShield);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + remainingDamage);
            
            var gainLevelValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, gainLevelValue - xpChange);


            //calculate Hit
            //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
            int attackRoll = RandomHelper.RollD20() + 7;

            if (attackRoll >= enemyAC || !GameManager.Instance.StochasticWorld)
            {
                //there was an hit, enemy is destroyed, gain xp
                //disables the target object so that it can't be reused again
                worldModel.SetProperty(this.Target, false);
                worldModel.SetProperty(PropertyKeys.XP, xp + this.xpChange);
            }
        }

        public override float GetHValue(WorldModel worldModel)
        {
            return expectedHPChange - expectedXPChange + GetDuration(worldModel) / 2f;
        }
    }
}
