using Assets.Scripts.IAJ.Unity.Utils;
using System;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class SpeedUp : Action
    {
        public AutonomousCharacter Character { get; private set; }

        private int _durationTicks = 250;

        public SpeedUp(AutonomousCharacter character) : base("SpeedUp")
        {
            this.Character = character;
        }
        
        public override float GetGoalChange(Goal goal)
        {
            float change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                change -= (goal.ChangeRate / 2) * this._durationTicks;
            }
            return change;
        }
        
        public override bool CanExecute()
        {
            return base.CanExecute() && Character.baseStats.Mana >= 5;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {
            int mana = (int)worldModel.GetProperty(Properties.MANA);
            return base.CanExecute() && mana >= 5;
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
                worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL) / 2 * this._durationTicks);
            worldModel.SetProperty(Properties.MANA, (int) worldModel.GetProperty(Properties.MANA) - 5);
        }
    }
}