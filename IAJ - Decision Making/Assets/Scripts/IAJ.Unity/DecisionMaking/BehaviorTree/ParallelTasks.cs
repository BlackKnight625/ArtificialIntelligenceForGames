namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree {
    public class ParallelTasks : CompositeTask {
        public override Result Run() {
            foreach (Task child in children) {
                // Ignore the returned result
                child.Run();
            }

            return Result.Success;
        }
    }
}