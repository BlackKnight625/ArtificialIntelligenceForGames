using System;
using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Formations {
    public class Anchor : NPC {
        private AnchorTree _anchorTree;

        public FormationManager FormationManager;

        private void Start() {
            _anchorTree = new AnchorTree(FormationManager);
        }

        private void FixedUpdate() {
            _anchorTree.Run();
        }
    }
}