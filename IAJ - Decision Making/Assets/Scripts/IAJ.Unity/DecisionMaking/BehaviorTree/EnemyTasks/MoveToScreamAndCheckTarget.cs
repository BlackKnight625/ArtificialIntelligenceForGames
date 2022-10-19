using Assets.Scripts.Game.NPCs;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class MoveToScreamAndCheckTarget : InterrupterUser {
        
        private readonly IsCharacterNearTarget _isCharacterNearPlayer;
        private readonly IsCharacterNearScream _isCharacterNearScream;
        private readonly MoveToScream _moveToScream;
        private readonly IsThereRecentScream _isThereRecentScream;

        public MoveToScreamAndCheckTarget(Monster character, GameObject target, float range, float screamReachedRange, Interrupter interrupter)
            : base(interrupter) {
            _isCharacterNearPlayer = new IsCharacterNearTarget(character, target, range);
            _isCharacterNearScream = new IsCharacterNearScream(character, screamReachedRange);
            _moveToScream = new MoveToScream(character, screamReachedRange);
            _isThereRecentScream = new IsThereRecentScream();
        }

        public override Result Run() {
            if (_isCharacterNearPlayer.Run() == Result.Success) {
                // Orc encountered player while moving towards the scream
                stopInterrupting();
                return Result.Failure;
            }

            Result screamResult = _isThereRecentScream.Run();
                
            if (screamResult == Result.Failure) {
                // There was a scream too long ago
                stopInterrupting();
                return Result.Failure;
            }
            
            Result nearScreamResult = _isCharacterNearScream.Run();

            if (nearScreamResult == Result.Success) {
                stopInterrupting();

                // Reached the scream
                return Result.Success;
            }
            
            interrupt();
            
            Result moveResult = _moveToScream.Run();

            if (moveResult != Result.Running) {
                stopInterrupting();
            }

            return moveResult;
        }
    }
}