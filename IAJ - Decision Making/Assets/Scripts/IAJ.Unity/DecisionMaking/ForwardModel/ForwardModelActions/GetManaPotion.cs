using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, Disposable target) : base("GetManaPotion",character,target)
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
            return worldModel.GetProperty(PropertyKeys.MANA) < 10;
        }
        
        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.GetManaPotion(this.Target.gameObject);
        }
        
        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            return change;
        }
        
        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            int mana = worldModel.GetProperty(PropertyKeys.MANA);
            int gainedMana = getGainedMana(worldModel);
            worldModel.SetProperty(PropertyKeys.MANA,  (mana + gainedMana));
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target, false);
        }

        public int getGainedMana(WorldModel worldModel) {
            return 10 - worldModel.GetProperty(PropertyKeys.MANA);
        }
        
        public override float GetHValue(WorldModel worldModel) {
            return -getGainedMana(worldModel) + GetDuration(worldModel) / 2f;
        }
    }
}