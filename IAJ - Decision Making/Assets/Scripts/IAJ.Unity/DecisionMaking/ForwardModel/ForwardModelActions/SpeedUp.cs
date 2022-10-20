using Assets.Scripts.IAJ.Unity.Utils;
using System;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class SpeedUp : Action
    {
        public AutonomousCharacter Character { get; private set; }

        private int _manaCost = 5;

        public SpeedUp(AutonomousCharacter character) : base("SpeedUp")
        {
            this.Character = character;
        }
        
        public override float GetGoalChange(Goal goal)
        {
            float change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                change -= (goal.ChangeRate / 2) * AutonomousCharacter.SPEEDUP_INTERVAL_TICKS;
            }
            return change;
        }
        
        public override bool CanExecute()
        {
            return base.CanExecute() && Character.baseStats.Mana >= _manaCost;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {
            return base.CanExecute() && (int)worldModel.GetProperty(Properties.MANA) >= _manaCost;
        }

        public override void Execute()
        {
            SpeedUpTime speedUpTime = new SpeedUpTime(Character);
            base.Execute();
            GameManager.Instance.SpeedUp();
            GameManager.Instance.WorldChanged = true;
            Character.playSpeedUpSound();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL,
                worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL) -
                    AutonomousCharacter.SPEEDUP_INTERVAL_TICKS / 2f * 50); // Amount of seconds saved walking
            worldModel.SetProperty(Properties.MANA, (int) worldModel.GetProperty(Properties.MANA) - _manaCost);
        }

        public override float GetHValue(WorldModel worldModel) {
            return _manaCost - (AutonomousCharacter.SPEEDUP_INTERVAL_TICKS / 2f * 50) / 2f;
        }
    }
}