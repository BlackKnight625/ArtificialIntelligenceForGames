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
                change += 0.1f * AutonomousCharacter.RESTING_INTERVAL_TICKS;
            }
            return change;
        }
        
        public override bool CanExecute()
        {
            return base.CanExecute() && Character.baseStats.HP + 2 <= Character.baseStats.MaxHP;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {
            int hp = worldModel.GetProperty(PropertyKeys.HP);
            int maxHp = worldModel.GetProperty(PropertyKeys.MAXHP);
            return base.CanExecute() && hp + 2 <= maxHp;
        }

        public override void Execute()
        {
            Character.Resting = true;
            Character.StopPathfinding();
            PrayTime prayTime = new PrayTime(Character);
            base.Execute();
            GameManager.Instance.Pray();
            Character.playPraySound();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL,
                worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) - 2);
            
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL,
                worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL) 
                + 0.1f * AutonomousCharacter.RESTING_INTERVAL_TICKS); // 0.1f Change rate times ticks
            
            int hp = worldModel.GetProperty(PropertyKeys.HP);
            int maxHp = worldModel.GetProperty(PropertyKeys.MAXHP);
            if (hp + 2 <= maxHp)
            {
                worldModel.SetProperty(PropertyKeys.HP, hp + 2);
            }
            else
            {
                worldModel.SetProperty(PropertyKeys.HP, maxHp);
            }
            worldModel.SetProperty(PropertyKeys.TIME, worldModel.GetProperty(PropertyKeys.TIME) + 5);
        }

        public override float GetHValue(WorldModel worldModel) {
            return 0.5f; // -2 + 2.5f = duration / 2 = 0.5f
        }
    }
}