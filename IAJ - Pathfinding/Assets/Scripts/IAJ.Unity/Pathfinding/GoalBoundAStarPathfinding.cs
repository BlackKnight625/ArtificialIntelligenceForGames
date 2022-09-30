using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Grid;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class GoalBoundAStarPathfinding : NodeArrayAStarPathfinding
    {
        public enum Direction {
            North,
            South,
            West,
            East,
            None
        }
        
        // You can create a bounding box in several differente ways, this is simply suggestion
        // Goal Bounding Box for each Node  direction - Bounding limits: minX, maxX, minY, maxY
        public Dictionary<Vector2,Dictionary<Direction, Vector4>> goalBounds;

        public GoalBoundAStarPathfinding(IHeuristic heuristic) : base(heuristic)
        {
            goalBounds = new Dictionary<Vector2, Dictionary<Direction, Vector4>>();
        }

        public void MapPreprocess()
        {
           
            for (int i = 0; i < grid.getHeight(); i++)
            {
                for (int j = 0; j < grid.getWidth(); j++)
                {
                    NodeRecord node = grid.GetGridObject(j, i);
                    if (grid.GetGridObject(node.x, node.y).isWalkable)
                    {
                        Vector2 coords = new Vector2(node.x, node.y);
                        goalBounds.Add(coords, new Dictionary<Direction, Vector4>());
                        // Floodfill the grid for each direction..
                        FloodFill(node);
                        // Calculate the bounding box and repeat
                        //NORTH
                        if (i + 1 < grid.getHeight())
                        {
                            NodeRecord north = grid.GetGridObject(j, i);
                            if (north.isWalkable)
                                goalBounds[coords].Add(Direction.North, boundsCalculator(north));
                        }

                        //SOUTH
                        if (i - 1 >= 0)
                        {
                            NodeRecord south = grid.GetGridObject(j, i);
                            if (south.isWalkable)
                                goalBounds[coords].Add(Direction.South, boundsCalculator(south));
                        }

                        //EAST
                        if (j + 1 < grid.getWidth())
                        {
                            NodeRecord east = grid.GetGridObject(j, i);
                            if (east.isWalkable)
                                goalBounds[coords].Add(Direction.East, boundsCalculator(east));
                        }
                        //WEST
                        if (j - 1 >= 0)
                        {
                            NodeRecord west = grid.GetGridObject(j, i);
                            if (west.isWalkable)
                                goalBounds[coords].Add(Direction.West, boundsCalculator(west));
                        }
                        
                        Reset();
                    }
                }
            }
        }

        private Vector4 boundsCalculator(NodeRecord direction)
        {
            Vector4 bounds = new Vector4(direction.x, direction.x, direction.y, direction.y);
            foreach (NodeRecord nodeFromGrid in grid.getAll())
            {
                if (nodeFromGrid.direction != null && nodeFromGrid.isWalkable) 
                {
                    if (nodeFromGrid.direction.Equals(direction))
                    {
                        if (bounds.x > nodeFromGrid.x) bounds.x = nodeFromGrid.x;
                        if (bounds.y < nodeFromGrid.x) bounds.y = nodeFromGrid.x;
                        if (bounds.z > nodeFromGrid.y) bounds.z = nodeFromGrid.y;
                        if (bounds.w < nodeFromGrid.y) bounds.w = nodeFromGrid.y;
                    }
                }

            }
            return bounds;
        }

        // You can change the arguments of the following method....
        public void FloodFill(NodeRecord original)
        {
            
            NodeRecord currentNode;
            Closed.AddToClosed(original);
            // Quite similar to the A*Search method except the fact that there is no goal....so where does it stop?

            foreach (var pair in fillNeighbourListWithDirection(original)) {
                if (pair.Item1.isWalkable) {
                    pair.Item1.direction = pair.Item2;

                    pair.Item1.gCost = CalculateDistanceCost(original, pair.Item1);
                    pair.Item1.parent = original;
                
                    Open.AddToOpen(pair.Item1);
                }
            }
            
            // Do stuff...
            while (Open.CountOpen() > 0)
            {
                
                currentNode = Open.GetBestAndRemove();
                Closed.AddToClosed(currentNode);
                foreach (var adjacent in fillNeighbourList(currentNode))
                {
                    if (adjacent.isWalkable) {
                        ProcessChildNodeFill(currentNode, adjacent);
                    }
                }
            }
            //At the end it is important to "clean" the Open and Closed Set
            this.Open.Initialize();
            this.Closed.Initialize();
        }
        
        private void resetStatusAndDirection() {
            foreach (var nodeRecord in grid.getAll()) {
                nodeRecord.direction = Direction.None;
                nodeRecord.status = NodeStatus.Unvisited;
            }
        }

        protected void ProcessChildNodeFill(NodeRecord parentNode, NodeRecord childNode) {
            // Floodfill Dijkstra
            var distance = parentNode.gCost + CalculateDistanceCost(parentNode, childNode);

            if (childNode.Equals(Open.SearchInOpen(childNode))) {
                if (distance < parentNode.gCost) {
                    childNode.gCost = distance;
                    childNode.parent = parentNode;
                    childNode.direction = parentNode.direction;
                }
            }
            else if(!childNode.Equals(Closed.SearchInClosed(childNode))) {
                // Not opened and not closed

                childNode.gCost = distance;
                childNode.parent = parentNode;
                childNode.direction = parentNode.direction;
                
                Open.AddToOpen(childNode);
            }
        }


        // Checks is if node(x,Y) is in the node(startx, starty) bounding box for the direction: direction
        public override bool InsindeGoalBoundBox(int startX, int startY, int x, int y, Direction direction)
        {
            Vector2 vectorKey = new Vector2(startX, startY);
            
            if (!this.goalBounds.ContainsKey(vectorKey))
                return false;

            if (!this.goalBounds[vectorKey].ContainsKey(direction))
                return false;

            var box = this.goalBounds[vectorKey][direction];
            
            //This is very ugly
            if(box.x >= -1 && box.y >= -1 && box.z >= -1 && box.w >= -1)
                if (x >= box.x && x <= box.y && y >= box.z && y <= box.w)
                    return true;

            return false;
        }

        protected List<NodeRecord> fillNeighbourList(NodeRecord currentNode)
        {
            List<NodeRecord> neighbourList = new List<NodeRecord>();
            //WEST
            if (currentNode.x - 1 >= 0)         
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //EAST
            if (currentNode.x + 1 < grid.getWidth())
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));                
            //SOUTH
            if (currentNode.y - 1 >= 0)
                neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
            //NORTH
            if (currentNode.y + 1 < grid.getHeight())
                neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

            return neighbourList;
        }
        
        protected List<(NodeRecord, Direction)> fillNeighbourListWithDirection(NodeRecord currentNode)
        {
            List<(NodeRecord, Direction)> neighbourList = new List<(NodeRecord, Direction)>();
            //WEST
            if (currentNode.x - 1 >= 0)         
                neighbourList.Add((GetNode(currentNode.x - 1, currentNode.y), Direction.West));
            //EAST
            if (currentNode.x + 1 < grid.getWidth())
                neighbourList.Add((GetNode(currentNode.x + 1, currentNode.y), Direction.East));                
            //SOUTH
            if (currentNode.y - 1 >= 0)
                neighbourList.Add((GetNode(currentNode.x, currentNode.y - 1), Direction.South));
            //NORTH
            if (currentNode.y + 1 < grid.getHeight())
                neighbourList.Add((GetNode(currentNode.x, currentNode.y + 1), Direction.North));

            return neighbourList;
        }
        
        protected override List<NodeRecord> GetNeighbourList(NodeRecord currentNode)
        {
            List<NodeRecord> neighbourList = new List<NodeRecord>();
            
            if (currentNode.x - 1 >= 0)
            {
                //WEST
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, Direction.West))
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            }
            if (currentNode.x + 1 < grid.getWidth())
            {
                //EAST
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, Direction.East))
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            }
            if (currentNode.y - 1 >= 0)
            {
                //SOUTH
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, Direction.South))
                    neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
            }
            if (currentNode.y + 1 < grid.getHeight())
            {
                //NORTH
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, Direction.North))
                    neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
            }
            
            return neighbourList;
        }
    }
}
