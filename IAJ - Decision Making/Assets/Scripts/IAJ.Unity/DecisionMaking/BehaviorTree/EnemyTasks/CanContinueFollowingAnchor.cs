using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.Formations;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class CanContinueFollowingAnchor : Task {
        private FormationManager _formationManager;
        private Monster _monster;
        private IsCharacterNearTarget _isCharacterNearPlayer;

        public CanContinueFollowingAnchor(Monster character, GameObject player, float targetRange, FormationManager formationManager) {
            _formationManager = formationManager;
            _monster = character;

            _isCharacterNearPlayer = new IsCharacterNearTarget(character, player, targetRange);
        }

        public override Result Run() {
            if (Orc.ScreamLocation == null && _isCharacterNearPlayer.Run() == Result.Failure) {
                return Result.Success;
            }
            else {
                _formationManager.RemoveCharacter(_monster);
                
                return Result.Failure;
            }
        }
    }
}