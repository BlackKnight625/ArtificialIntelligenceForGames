using Assets.Scripts.Game.NPCs;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class MoveToTargetWhileClose : Task {

        private MoveTo _moveTo;
        private IsCharacterNearTarget _isCharacterNearTarget;

        public MoveToTargetWhileClose(Monster character, GameObject target,
            float arrivedRange, float giveUpRange) {

            _moveTo = new MoveTo(character, target, arrivedRange);
            _isCharacterNearTarget = new IsCharacterNearTarget(character, target, giveUpRange);
        }

        public override Result Run() {
            if (_isCharacterNearTarget.Run() == Result.Success) {
                // Character is near the target. Continue pursuing
                return _moveTo.Run();
            }
            else {
                // Character is far away from the target. Give up
                return Result.Failure;
            }
        }
    }
}