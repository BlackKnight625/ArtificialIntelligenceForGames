using Assets.Scripts.Game.NPCs;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class MoveToPatrolAndCheckTarget : Task {
        private Sequence _moveToPatrols;
        private IsCharacterNearTarget _isCharacterNearPlayer;

        public MoveToPatrolAndCheckTarget(Monster character, GameObject target,
            float targetRange, GameObject patrol1, GameObject patrol2, float patrolRange) {
            _moveToPatrols = new Sequence(
                new MoveTo(character, patrol1, patrolRange),
                        new MoveTo(character, patrol2, patrolRange)
                );
            _isCharacterNearPlayer = new IsCharacterNearTarget(character, target, targetRange);
        }

        public override Result Run() {
            if (_isCharacterNearPlayer.Run() == Result.Success) {
                // This character is near the target. Failing so that the behaviour tree may pursue the target
                return Result.Failure;
            }
            else {
                // Character is not near target. Patrol.
                return _moveToPatrols.Run();
            }
        }
    }
}