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
        private static Orc _whoScreamed;

        public GameObject patrol1;
        public GameObject patrol2;
        public AudioSource scream;
        public Renderer orcRenderer;

        public Vector3 growthPerTick = new Vector3(0.02f, 0.02f, 0.02f);
        public int changeTicks = 12;
        public int maxChanges = 4;
        public float redPerTick = 0.03f;
        public int screamCooldownTicks = 50 * 4;
        
        private int _currentChanges = 0;
        private int _growing = 1;
        private int _currentChangeTick = 0;
        private int _screamCooldown = 0;
        
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

        public override void Start() {
            base.Start();
            
            scream = GetComponent<AudioSource>();
            orcRenderer = GetComponentInChildren<Renderer>();
        }

        public override void foundPlayer(GameObject player) {
            if (ScreamLocation == null) {
                _whoScreamed = this;
                
                // Creating the screaming location
                ScreamLocation = new GameObject();

                ScreamLocation.transform.position = character.transform.position;
            
                // Playing a sound
                scream.Play();

                GameManager.Instance.StartCoroutine(screamRoutine());
            }
        }

        private IEnumerator screamRoutine() {
            // Making the orc change size and change color
            while (_currentChanges < maxChanges) {
                if (this != null) {
                    transform.localScale += growthPerTick * _growing;
                    orcRenderer.material.color += new Color(redPerTick * _growing, 0, 0);
                }
                
                _currentChangeTick++;

                if (_currentChangeTick % changeTicks == 0) {
                    // Flipping the growth/coloring
                    _growing *= -1;

                    if (_currentChangeTick % (changeTicks * 2) == 0) {
                        // The character has returned to its original form
                        _currentChanges++;
                    }
                }
                
                yield return new WaitForFixedUpdate();
            }

            // Resetting 
            _growing = 1;
            _currentChanges = 0;
            _currentChangeTick = 0;
            
            // Setting the screaming back to null after a while
            while (_screamCooldown < screamCooldownTicks) {
                _screamCooldown++;
                
                yield return new WaitForFixedUpdate();
            }

            _screamCooldown = 0;

            ScreamLocation = null;
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
