using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Grid;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class NodeArrayAStarPathfinding : AStarPathfinding
    {
        private static int index = 0;
        protected NodeRecordArray NodeRecordArray { get; set; }

        public NodeArrayAStarPathfinding(IHeuristic heuristic) : base(null, null, heuristic)
        {
            grid = new Grid<NodeRecord>((Grid<NodeRecord> global, int x, int y) => new NodeRecord(x, y, index++));
            this.InProgress = false;
            this.Heuristic = heuristic;
            this.NodesPerSearch = uint.MaxValue;
            this.NodeRecordArray = new NodeRecordArray(grid.getAll());
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;

        }
       
        // In Node Array A* the only thing that changes is how you process the child node, the search occurs the exact same way so you can the parent's method
        protected override void ProcessChildNode(NodeRecord parentNode, NodeRecord neighbourNode)
        {
            // TODO implement
            float F;
            float G;
            float H;

            var childNode = neighbourNode;

            childNode.gCost = parentNode.gCost + CalculateDistanceCost(parentNode, childNode);
            childNode.hCost = Heuristic.H(childNode, GoalNode);
            childNode.CalculateFCost();

            if (neighbourNode.status == NodeStatus.Unvisited)
            {
                neighbourNode.gCost = childNode.gCost;
                neighbourNode.hCost = childNode.hCost;
                neighbourNode.CalculateFCost();
            }
            else if (neighbourNode.status == NodeStatus.Open)
            {
                if (childNode.fCost < neighbourNode.fCost)
                {
                    neighbourNode.gCost = childNode.gCost;
                    neighbourNode.hCost = childNode.hCost;
                    neighbourNode.CalculateFCost();
                }
            }
            else if (neighbourNode.status == NodeStatus.Closed)
            {
                if (childNode.fCost < neighbourNode.fCost)
                {
                    Open.AddToOpen(childNode);
                }
            }
            updateNodeStatus(childNode, NodeStatus.Open);
        }
               
            
        }


       
}
