using Assets.Scripts.Game.NPCs;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class MoveToPatrolAndCheckTarget : Task {
        private MoveTo _moveToPatrol1;
        private MoveTo _moveToPatrol2;
        private IsCharacterNearTarget _isCharacterNearPlayer;
        private bool _togglePatrol = true; // Allows the character to toggle between patrol 1 and 2

        public MoveToPatrolAndCheckTarget(Monster character, GameObject target,
            float targetRange, GameObject patrol1, GameObject patrol2, float patrolRange) {
            _moveToPatrol1 = new MoveTo(character, patrol1, patrolRange);
            _moveToPatrol2 = new MoveTo(character, patrol2, patrolRange);
            _isCharacterNearPlayer = new IsCharacterNearTarget(character, target, targetRange);
        }

        public override Result Run() {
            if (_isCharacterNearPlayer.Run() == Result.Success) {
                // This character is near the target. Failing so that the behaviour tree may pursue the target
                return Result.Failure;
            }
            else {
                // Character is not near target. Patrol.
                
                Result result;
                
                result = _togglePatrol ? _moveToPatrol1.Run() : _moveToPatrol2.Run();
                
                if (result == Result.Success) {
                    // Reached the patrol point. Switching patrol point
                    _togglePatrol = !_togglePatrol;
                        
                    return Result.Success;
                }
                else {
                    return result;
                }
            }
        }
    }
}