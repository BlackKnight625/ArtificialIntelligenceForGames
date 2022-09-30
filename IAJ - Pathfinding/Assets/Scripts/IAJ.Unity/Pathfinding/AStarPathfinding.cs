using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Grid;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using System.Runtime.CompilerServices;
using System;
using UnityEditor.AssetImporters;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    [Serializable]
    public class AStarPathfinding
    {
        // Cost of moving through the grid
        protected const float MOVE_STRAIGHT_COST = 1;
        protected const float MOVE_DIAGONAL_COST = 1.4f;
        public Grid<NodeRecord> grid { get; set; }
        public uint NodesPerSearch { get; set; }
        public uint TotalProcessedNodes { get; protected set; }
        public int MaxOpenNodes { get; protected set; }
        public float TotalProcessingTime { get; set; }
        public bool InProgress { get; set; }
        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }
         public IHeuristic Heuristic { get; protected set; }

        public NodeRecord GoalNode { get; set; }
        public NodeRecord StartNode { get; set; }
        public int StartPositionX { get; set; }
        public int StartPositionY { get; set; }
        public int GoalPositionX { get; set; }
        public int GoalPositionY { get; set; }

        public AStarPathfinding(IOpenSet open, IClosedSet closed, IHeuristic heuristic)
        {
            grid = new Grid<NodeRecord>((Grid<NodeRecord> global, int x, int y) => new NodeRecord(x, y));
            this.Open = open;
            this.Closed = closed;
            this.InProgress = false;
            this.Heuristic = heuristic;
            this.NodesPerSearch = 100; //by default we process all nodes in a single request, but you should change this

        }
        public virtual void InitializePathfindingSearch(int startX, int startY, int goalX, int goalY)
        {
            Reset();
            
            this.StartPositionX = startX;
            this.StartPositionY = startY;
            this.GoalPositionX = goalX;
            this.GoalPositionY = goalY;
            this.StartNode = grid.GetGridObject(StartPositionX, StartPositionY);
            this.GoalNode = grid.GetGridObject(GoalPositionX, GoalPositionY);

            //if it is not possible to quantize the positions and find the corresponding nodes, then we cannot proceed
            if (this.StartNode == null || this.GoalNode == null) return;
            // Reset debug and relevat variables here
            this.InProgress = true;
            this.TotalProcessedNodes = 0;
            this.TotalProcessingTime = 0.0f;
            this.MaxOpenNodes = 0;

            // Instead of creating a copy of the StartNode, we chose to use the already existing one
            StartNode.gCost = 0;
            StartNode.hCost = this.Heuristic.H(this.StartNode, this.GoalNode);

            StartNode.CalculateFCost();
            this.Open.Initialize();
            this.Open.AddToOpen(StartNode);
            this.Closed.Initialize();
        }
        public virtual bool Search(out List<NodeRecord> solution, bool returnPartialSolution = false) {

            var ProcessedNodes = 0;
            NodeRecord CurrentNode;
            
            while (Open.CountOpen() > 0) {
                if (ProcessedNodes >= NodesPerSearch) {
                    // The search is over
                    if (returnPartialSolution) {
                        // Returning this partial solution
                        solution = CalculatePath(Open.PeekBest());
                        InProgress = false;
                        return true;
                    }
                    else {
                        // Returning temporary solution
                        solution = CalculatePath(Open.PeekBest());
                        
                        return false;
                    }
                }
                
                CurrentNode = Open.GetBestAndRemove();

                if (CurrentNode.Equals(GoalNode)) {
                    solution = CalculatePath(CurrentNode);
                    InProgress = false;
                    
                    return true;
                }
                
                Open.RemoveFromOpen(CurrentNode);
                Closed.AddToClosed(CurrentNode);
                
                // Changing the color of the node
                grid.SetGridObject(CurrentNode.x, CurrentNode.y, CurrentNode);

                foreach (var adjacent in GetNeighbourList(CurrentNode)) {
                    ProcessChildNode(CurrentNode, adjacent);
                    
                    ProcessedNodes++;
                    TotalProcessedNodes++;
                }
            }

            InProgress = false;
            solution = null;
            return false;
        
        }

        protected virtual void ProcessChildNode(NodeRecord parentNode, NodeRecord childNode) {
            var distance = parentNode.gCost + CalculateDistanceCost(parentNode, childNode);
            var newCost = distance + childNode.hCost;

            if (childNode.Equals(Closed.SearchInClosed(childNode))) {
                if (newCost < childNode.fCost) {
                    childNode.gCost = distance;
                    childNode.parent = parentNode;
                    
                    Closed.RemoveFromClosed(childNode);
                    Open.AddToOpen(childNode);
                }
            }
            else if (childNode.Equals(Open.SearchInOpen(childNode))) {
                if (newCost < childNode.fCost) {
                    childNode.gCost = distance;
                    childNode.parent = parentNode;
                }
            }
            else {
                childNode.gCost = distance;
                childNode.parent = parentNode;
                childNode.hCost = Heuristic.H(childNode, GoalNode);
                Open.AddToOpen(childNode);
            }

            childNode.direction = parentNode.direction;
            
            if (childNode.direction == null) childNode.direction = childNode;
            childNode.CalculateFCost();
            
            // Letting the Event Handler know that this node's state possibly changed, so that its color may be updated
            grid.SetGridObject(childNode.x, childNode.y, childNode);

            MaxOpenNodes = Math.Max(MaxOpenNodes, Open.CountOpen());
        }
        
        public virtual bool InsindeGoalBoundBox(int startX, int startY, int x, int y, string direction)
        {
            return false;
        }

        protected float CalculateDistanceCost(NodeRecord a, NodeRecord b)
        {
            // Math.abs is quite slow, thus we try to avoid it
            int xDistance = 0;
            int yDistance = 0;
            int remaining = 0;

            if (b.x > a.x)
                xDistance = b.x - a.x;
            else xDistance = a.x - b.x;

            if (b.y > a.y)
                yDistance = b.y - a.y;
            else yDistance = a.y - b.y;

            if (yDistance > xDistance)
                remaining = yDistance - xDistance;
            else remaining = xDistance - yDistance;

            // Diagonal Cost * Diagonal Size + Horizontal/Vertical Cost * Distance Left
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        // You'll need to use this method during the Search, to get the neighboors
        protected virtual List<NodeRecord> GetNeighbourList(NodeRecord currentNode)
        {
            List<NodeRecord> neighbourList = new List<NodeRecord>();

            if(currentNode.x - 1 >= 0)
            {
                // Left
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
                //Left down
                if(currentNode.y - 1 >= 0)
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                //Left up
                if (currentNode.y + 1 < grid.getHeight())
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
            if (currentNode.x + 1 < grid.getWidth())
            {
                // Right
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
                //Right down
                if (currentNode.y - 1 >= 0)
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                //Right up
                if (currentNode.y + 1 < grid.getHeight())
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
            // Down
            if (currentNode.y - 1 >= 0)
                neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
            //Up
            if (currentNode.y + 1 < grid.getHeight())
                neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

            neighbourList.RemoveAll(x => !x.isWalkable);

            return neighbourList;
        }

        public NodeRecord GetNode(int x, int y)
        {
            return grid.GetGridObject(x, y);
        }


        // Method to calculate the Path, starts from the end Node and goes up until the beginning
        public List<NodeRecord> CalculatePath(NodeRecord endNode)
        {
            List<NodeRecord> path = new List<NodeRecord>();
            path.Add(endNode);

            NodeRecord currentNode = endNode;

            while (currentNode.parent != null) {
                currentNode = currentNode.parent;
                
                path.Add(currentNode);
            }

            path.Reverse();
            
            return path;
        }

        public void Reset() {
            // Resetting all nodes that have been tampered with
            foreach(NodeRecord record in Closed.All()) {
                record.Reset();
            }
            foreach(NodeRecord record in Open.All()) {
                record.Reset();
            }
        }
    }
}
