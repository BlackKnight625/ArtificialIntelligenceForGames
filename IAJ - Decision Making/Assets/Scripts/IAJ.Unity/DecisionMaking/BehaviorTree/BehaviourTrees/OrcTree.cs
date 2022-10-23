using System.Collections.Generic;
using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees {
    public class OrcTree : ParallelTasks {

        public OrcTree(Monster character, GameObject target, GameObject patrol1, GameObject patrol2) : base() {
            Interrupter interrupter = new Interrupter(
                new Selector(
                    new Sequence(
                        new IsCharacterNearTarget(character, target, character.enemyStats.AwakeDistance),
                        new MoveToTargetWhileClose(character, target, character.enemyStats.WeaponRange, character.enemyStats.AwakeDistance),
                        new LightAttack(character)
                    ),
                    new MoveToPatrolAndCheckTarget(character, target,
                        character.enemyStats.AwakeDistance, patrol1, patrol2, character.enemyStats.WeaponRange)
                )
            );

            Sequence screamSequence = new Sequence(
                new IsThereRecentScream(),
                new MoveToScreamAndCheckTarget(character, target, character.enemyStats.AwakeDistance,
                    character.enemyStats.WeaponRange, interrupter)
            );
            
            setTasks(interrupter, screamSequence);
        }
    }
}