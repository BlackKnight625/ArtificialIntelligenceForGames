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
                this.enemySimpleDamage = 3;
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

            int hp = (int)worldModel.GetProperty(Properties.HP);
            int shieldHp = (int)worldModel.GetProperty(Properties.ShieldHP);
            int mana = (int)worldModel.GetProperty(Properties.MANA);
            int xp = (int)worldModel.GetProperty(Properties.XP);

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
                worldModel.SetProperty(Properties.HP, remainingHP);
            }

            worldModel.SetProperty(Properties.ShieldHP, remainingShield);
            worldModel.SetProperty(Properties.MANA, mana - 2);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + remainingDamage);


            //calculate Hit
            //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
            int attackRoll = RandomHelper.RollD20() + 7;

            if (attackRoll >= enemyAC || !GameManager.Instance.StochasticWorld)
            {
                //there was an hit, enemy is destroyed, gain xp
                //disables the target object so that it can't be reused again
                worldModel.SetProperty(this.Target.name, false);
                worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            }
        }

        public override float GetHValue(WorldModel worldModel)
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            
            if (hp > this.expectedHPChange)
            {
                return base.GetHValue(worldModel)/1.5f;
            }
            return 10.0f;
        }
    }
}