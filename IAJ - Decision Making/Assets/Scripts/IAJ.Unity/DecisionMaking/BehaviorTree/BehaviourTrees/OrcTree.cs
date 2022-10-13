using System.Collections.Generic;
using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees {
    class OrcTree : Sequence {
        public OrcTree(Monster character, GameObject target)
        {
            // To create a new tree you need to create each branck which is done using the constructors of different tasks
            // Additionally it is possible to create more complex behaviour by combining different tasks and composite tasks...
            this.children = new List<Task>()
            {
                new IsCharacterNearTarget(character, target, character.enemyStats.AwakeDistance),
                new MoveTo(character, target, character.enemyStats.WeaponRange),
                new LightAttack(character)
            };
        }
    }
}