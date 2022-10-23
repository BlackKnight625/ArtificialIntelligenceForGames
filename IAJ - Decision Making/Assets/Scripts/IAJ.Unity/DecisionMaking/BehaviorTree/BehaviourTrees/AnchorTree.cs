using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using Assets.Scripts.IAJ.Unity.Formations;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees {
    public class AnchorTree : Sequence {

        public AnchorTree(FormationManager formationManager) {
            GameObject patrol1Point = new GameObject("Anchor Patrol 1");
            GameObject patrol2Point = new GameObject("Anchor Patrol 2");

            patrol1Point.transform.position = new Vector3(7.5f, 0, 36);
            patrol2Point.transform.position = new Vector3(62, 0, 44.5f);
            
            MoveTo patrol1 = new MoveTo(formationManager.Anchor, patrol1Point, 4);
            MoveTo patrol2 = new MoveTo(formationManager.Anchor, patrol2Point, 4);
            
            setTasks(patrol1, patrol2);
        }
    }
}