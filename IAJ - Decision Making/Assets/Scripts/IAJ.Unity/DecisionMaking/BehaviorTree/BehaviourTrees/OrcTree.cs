using System.Collections.Generic;
using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees {
    class OrcTree : Selector {
        public OrcTree(Monster character, GameObject target, GameObject patrol1, GameObject patrol2) : base(
            new List<Task>() {
                new Sequence(new List<Task>() {
                    new IsCharacterNearTarget(character, target, character.enemyStats.AwakeDistance),
                    new MoveToTargetWhileClose(character, target, character.enemyStats.WeaponRange, character.enemyStats.AwakeDistance),
                    new LightAttack(character)
                }),
                new MoveToPatrolAndCheckTarget(character, target,
                    character.enemyStats.AwakeDistance, patrol1, patrol2, character.enemyStats.WeaponRange)
            }
        ) { }
    }
}