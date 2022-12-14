using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Game
{
    public class NPC : MonoBehaviour
    {
        [Serializable]
        public struct Stats
        {
            public string Name;
            public int HP;
            public int ShieldHP;
            public int MaxHP;
            public int Mana;
            public int XP;
            public float Time;
            public int Money;
            public int Level;

        }

        protected GameObject character;

        // Pathfinding
        public UnityEngine.AI.NavMeshAgent navMeshAgent;
        private Vector3 previousTarget;

        public Stats baseStats;


        void Awake()
        {
            previousTarget = new Vector3(0.0f, 0.0f, 0.0f);
            this.character = this.gameObject;
            navMeshAgent = this.GetComponent<NavMeshAgent>();
        }

        public virtual void foundPlayer(GameObject player) {
            // Do nothing by default
        }

        public virtual bool notifyFoundPlayer() {
            return false;
        }

        #region Navmesh Pathfinding Methods

        public void StartPathfinding(Vector3 targetPosition)
        {
            //if the targetPosition received is the same as a previous target, then this a request for the same target
            //no need to redo the pathfinding search
            if (!this.previousTarget.Equals(targetPosition))
            {

                this.previousTarget = targetPosition;

                navMeshAgent.SetDestination(targetPosition);
            }
        }

        public void StopPathfinding()
        {
            navMeshAgent.isStopped = true;
        }

        private readonly Dictionary<(Vector3, Vector3), float> _cache = new();

        // Simple way of calculating distance left to target using Unity's navmesh
        public float GetDistanceToTarget(Vector3 originalPosition, Vector3 targetPosition) {
            (Vector3, Vector3) cacheKey = (originalPosition, targetPosition);
            
            try {
                return _cache[cacheKey];
            }
            catch (KeyNotFoundException) {
                // Cache does not contain result for the given input
                var distance = 0.0f;

                NavMeshPath result = new NavMeshPath();
                var r = NavMesh.CalculatePath(originalPosition, targetPosition, NavMesh.AllAreas, result);
                //var r = navMeshAgent.CalculatePath(targetPosition, result);
                if (r == true)
                {
                    var currentPosition = originalPosition;
                    foreach (var c in result.corners)
                    {
                        //Rough estimate, it does not account for shortcuts so we have to multiply it
                        distance += Vector3.Distance(currentPosition, c) * 0.65f;
                        currentPosition = c;
                    }
                }
                else {
                    //Default value
                    distance = 100;
                }

                _cache[cacheKey] = distance;

                return distance;
            }
        }

        #endregion

    }
}
