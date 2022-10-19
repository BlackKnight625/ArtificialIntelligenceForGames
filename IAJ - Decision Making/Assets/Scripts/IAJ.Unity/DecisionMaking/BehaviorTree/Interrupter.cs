using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree {
    public class Interrupter : Task {

        private readonly Task _task;
        
        public bool Interrupt { get; set; }
        
        
        public Interrupter(Task task) {
            Interrupt = false;

            _task = task;
        }

        public override Result Run() {
            if (Interrupt) {
                return Result.Failure;
            }
            else {
                return _task.Run();
            }
        }
    }
}