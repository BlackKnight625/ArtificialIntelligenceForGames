using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using Assets.Scripts.IAJ.Unity.Formations;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees {
    
    public class OrcInFormationTree : Selector {
        public OrcInFormationTree(Monster character, GameObject target, GameObject patrol1, GameObject patrol2, FormationManager formationManager) 
            : base(
                new UntilFail(new CanContinueFollowingAnchor(character, target, character.enemyStats.WeaponRange, formationManager)),
                new OrcTree(character, target, patrol1, patrol2)
                ) {}
    }
}