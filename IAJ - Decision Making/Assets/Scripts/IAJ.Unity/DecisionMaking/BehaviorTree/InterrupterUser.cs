namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree {
    public class InterrupterUser : Task {

        private readonly Interrupter _interrupter;

        public InterrupterUser(Interrupter interrupter) {
            _interrupter = interrupter;
        }

        public void interrupt() {
            _interrupter.Interrupt = true;
        }

        public void stopInterrupting() {
            _interrupter.Interrupt = false;
        }
    }
}