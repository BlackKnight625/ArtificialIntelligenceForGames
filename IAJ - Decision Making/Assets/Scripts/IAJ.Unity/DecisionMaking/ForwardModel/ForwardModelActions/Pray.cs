using Assets.Scripts.IAJ.Unity.Utils;
using System;
using Assets.Scripts.Game;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class Pray : Action
    {
        public AutonomousCharacter Character { get; private set; }

        public Pray(AutonomousCharacter character) : base("Pray")
        {
            this.Character = character;
        }

        public override float GetGoalChange(Goal goal)
        {
            float change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change -= 2;
            }
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                change += 5;
            }
            return change;
        }
        
        public override bool CanExecute()
        {
            return base.CanExecute() && Character.baseStats.HP + 2 <= Character.baseStats.MaxHP;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {
            int hp = (int)worldModel.GetProperty(Properties.HP);
            int maxHp = (int)worldModel.GetProperty(Properties.MAXHP);
            return base.CanExecute() && hp + 2 <= maxHp;
        }

        public override void Execute()
        {
            Character.Resting = true;
            Character.StopPathfinding();
            PrayTime prayTime = new PrayTime(Character);
            base.Execute();
            GameManager.Instance.Pray();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL,
                worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - 2);
            
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL,
                worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL) + 5);
            
            int hp = (int)worldModel.GetProperty(Properties.HP);
            if (hp + 2 <= (int) worldModel.GetProperty(Properties.MAXHP))
            {
                worldModel.SetProperty(Properties.HP, hp + 2);
            }
            else
            {
                worldModel.SetProperty(Properties.HP, Properties.MAXHP);
            }
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 
                worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) +
                (int) worldModel.GetProperty(Properties.HP) + 2);
            worldModel.SetProperty(Properties.TIME, (float) worldModel.GetProperty(Properties.TIME) + 5);
        }
    }
}