using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Game;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class LevelUp : Action
    {
        public AutonomousCharacter Character { get; private set; }

        public LevelUp(AutonomousCharacter character) : base("LevelUp")
        {
            this.Character = character;
        }

        public override bool CanExecute()
        {
            var level = Character.baseStats.Level;
            var xp = Character.baseStats.XP;

            return xp >= level * 10;
        }
        

        public override bool CanExecute(WorldModel worldModel)
        {
            int xp = worldModel.GetProperty(PropertyKeys.XP);
            int level = worldModel.GetProperty(PropertyKeys.LEVEL);

            return xp >= level * 10;
        }

        public override void Execute()
        {
            GameManager.Instance.LevelUp();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            int maxHP = worldModel.GetProperty(PropertyKeys.MAXHP);
            int level = worldModel.GetProperty(PropertyKeys.LEVEL);

            worldModel.SetProperty(PropertyKeys.LEVEL, level + 1);
            worldModel.SetProperty(PropertyKeys.MAXHP, maxHP + 10);
            worldModel.SetProperty(PropertyKeys.XP, 0);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, 0);
        }

        public override float GetGoalChange(Goal goal)
        {
            float change = 0.0f;

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change = -goal.InsistenceValue;
            }

            return change;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //you would be dumb not to level up if possible
            return -100.0f;
        }
    }
}
