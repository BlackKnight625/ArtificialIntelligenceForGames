using Assets.Scripts.Game;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : Action
    {
        AutonomousCharacter character;
        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
        {
            this.character = character;
        }
        
        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.ShieldOfFaith();
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()|| this.character.baseStats.Mana < 5 || this.character.baseStats.ShieldHP >= 5)
                return false;
            return true;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {            
            if (!base.CanExecute(worldModel) || worldModel.GetProperty(PropertyKeys.MANA) < 5
                                             || worldModel.GetProperty(PropertyKeys.ShieldHP) >= 5)
                return false;
            return true;
        }
        
        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            
            worldModel.SetProperty(PropertyKeys.ShieldHP, 5);
            worldModel.SetProperty(PropertyKeys.MANA, worldModel.GetProperty(PropertyKeys.MANA) - 5);
            
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 
                worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) +
                worldModel.GetProperty(PropertyKeys.ShieldHP) - 5);
        }
        
        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += GameManager.Instance.Character.baseStats.ShieldHP - 5;
            }

            return change;
        }

        public override float GetHValue(WorldModel worldModel) {
            return 0; // 5 mana - 5 hp
        }
    }
}