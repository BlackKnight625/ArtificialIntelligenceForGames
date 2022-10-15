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
            if (!base.CanExecute(worldModel) || (int)(worldModel.GetProperty(Properties.MANA)) < 5
                                             || (int)(worldModel.GetProperty(Properties.ShieldHP)) >= 5)
                return false;
            return true;
        }
        
        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            
            worldModel.SetProperty(Properties.ShieldHP, 5);
            worldModel.SetProperty(Properties.MANA, (int) worldModel.GetProperty(Properties.MANA) - 5);
            
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 
                worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL) +
                (int) worldModel.GetProperty(Properties.ShieldHP) - 5);
        }
    }
}