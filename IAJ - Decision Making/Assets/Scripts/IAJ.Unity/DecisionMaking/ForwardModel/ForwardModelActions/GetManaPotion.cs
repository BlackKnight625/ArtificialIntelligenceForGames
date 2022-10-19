using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion",character,target)
        {
        }
        
        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.Mana < 10;
        }
        
        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute()) return false;
            return (int) worldModel.GetProperty(Properties.MANA) < 10;
        }
        
        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.GetManaPotion(this.Target);
        }
        
        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            return change;
        }
        
        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            int mana = (int) worldModel.GetProperty(Properties.MANA);
            int gainedMana = (int) GetHValue(worldModel);
            worldModel.SetProperty(Properties.MANA, (int)(mana + gainedMana));
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }
        
        public override float GetHValue(WorldModel worldModel)
        {
            return 10 - (int) worldModel.GetProperty(Properties.MANA);
        }
    }
}