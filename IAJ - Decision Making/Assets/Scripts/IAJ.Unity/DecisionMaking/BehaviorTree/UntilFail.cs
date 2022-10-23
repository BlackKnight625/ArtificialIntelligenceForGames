namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree {
    public class UntilFail : Task {
        private readonly Task _task;
        private bool _failed = false;

        public UntilFail(Task task) {
            _task = task;
        }

        public override Result Run() {
            if (_failed) {
                return Result.Failure;
            }
            else {
                Result result = _task.Run();

                if (result == Result.Failure) {
                    _failed = true;
                }
                else if (result == Result.Success) {
                    // Since it's until failure, this returns Running
                    return Result.Running;
                }

                return result;
            }
        }
    }
}