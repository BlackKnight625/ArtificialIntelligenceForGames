using Assets.Scripts.Game.NPCs;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class IsThereRecentScream : Task {
        
        public IsThereRecentScream() {}
        
        public override Result Run() {
            if (Orc.ScreamLocation == null) {
                return Result.Failure;
            }
            else {
                return Result.Success;
            }
        }
    }
}