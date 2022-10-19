using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree {
    public class Interruptor : Task {

        private Task _task;
        
        public bool Interrupt { get; set; }
        
        
        public Interruptor(Task task) {
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