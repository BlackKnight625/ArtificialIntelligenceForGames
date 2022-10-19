using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine.AI;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees;
using Assets.Scripts.IAJ.Unity.Formations;
using System.Collections.Generic;

namespace Assets.Scripts.Game.NPCs
{

    public class Orc : Monster {

        private static GameObject _screamLocation;

        public GameObject patrol1;
        public GameObject patrol2;
        public AudioSource scream;
        
        public static GameObject ScreamLocation {
            get => _screamLocation;
            set {
                if (_screamLocation != null) {
                    Destroy(_screamLocation);
                }

                _screamLocation = value;
            }
        }
        
        public Orc()
        {
            this.enemyStats.Type = "Orc";
            this.enemyStats.XPvalue = 10;
            this.enemyStats.AC = 14;
            this.baseStats.HP = 15;
            this.DmgRoll = () => RandomHelper.RollD10() + 2;
            this.enemyStats.SimpleDamage = 5;
            this.enemyStats.AwakeDistance = 10;
            this.enemyStats.WeaponRange = 3;
        }
        
        

        public override void foundPlayer(GameObject player) {
            // Creating the screaming location
            ScreamLocation = new GameObject();

            ScreamLocation.transform.position = character.transform.position;
            
            // Playing a sound
            scream.Play();
        }

        public override bool notifyFoundPlayer() {
            return true;
        }
        
        

        public override void InitializeBehaviourTree()
        {
            this.BehaviourTree = new OrcTree(this, this.Target, patrol1, patrol2);
        }
    }
}
