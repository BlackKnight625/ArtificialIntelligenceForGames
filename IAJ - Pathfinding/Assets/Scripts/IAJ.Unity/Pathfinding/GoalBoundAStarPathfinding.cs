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
        // You can create a bounding box in several differente ways, this is simply suggestion
        // Goal Bounding Box for each Node  direction - Bounding limits: minX, maxX, minY, maxY
        public Dictionary<Vector2,Dictionary<string, Vector4>> goalBounds;

        public GoalBoundAStarPathfinding(IOpenSet open, IClosedSet closed, IHeuristic heuristic) : base(open, closed, heuristic)
        {
            goalBounds = new Dictionary<Vector2, Dictionary<string, Vector4>>();
        }

        public void MapPreprocess()
        {
           
            for (int i = 0; i < grid.getHeight(); i++)
            {
                for (int j = 0; j < grid.getWidth(); j++)
                {
                    NodeRecord node = new NodeRecord(j, i);
                    if (grid.GetGridObject(node.x, node.y).isWalkable)
                    {
                        Vector2 coords = new Vector2(node.x, node.y);
                        goalBounds.Add(coords, new Dictionary<string, Vector4>());
                        // Floodfill the grid for each direction..
                        FloodFill(node);
                        // Calculate the bounding box and repeat
                        //NORTH
                        if (i + 1 < grid.getHeight())
                        {
                            NodeRecord north = new NodeRecord(j, i + 1);
                            if (grid.GetGridObject(north.x, north.y).isWalkable)
                                goalBounds[coords].Add("NORTH", boundsCalculator(north));
                        }

                        //SOUTH
                        if (i - 1 >= 0)
                        {
                            NodeRecord south = new NodeRecord(j, i - 1);
                            if (grid.GetGridObject(south.x, south.y).isWalkable)
                                goalBounds[coords].Add("SOUTH", boundsCalculator(south));
                        }

                        //EAST
                        if (j + 1 < grid.getWidth())
                        {
                            NodeRecord east = new NodeRecord(j + 1, i);
                            if (grid.GetGridObject(east.x, east.y).isWalkable)
                                goalBounds[coords].Add("EAST", boundsCalculator(east));
                        }
                        //WEST
                        if (j - 1 >= 0)
                        {
                            NodeRecord west = new NodeRecord(j - 1, i);
                            if (grid.GetGridObject(west.x, west.y).isWalkable)
                                goalBounds[coords].Add("WEST", boundsCalculator(west));
                        }

                        foreach (NodeRecord nodeFromGrid in grid.getAll())
                        {
                            nodeFromGrid.Reset();
                        }
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
            this.Open.AddToOpen(original);
            // Quite similar to the A*Search method except the fact that there is no goal....so where does it stop?

            // Do stuff...
            while (Open.CountOpen() > 0)
            {
                
                currentNode = Open.GetBestAndRemove();
                Closed.AddToClosed(currentNode);
                Debug.Log("AAAAAAAAAAAAAA");
                foreach (var adjacent in fillNeighbourList(currentNode))
                {
                    if (adjacent.isWalkable)
                        ProcessChildNode(currentNode, adjacent);
                }
            }
            //At the end it is important to "clean" the Open and Closed Set
            this.Open.Initialize();
            this.Closed.Initialize();
            Debug.Log("BBBBBBBBBB");
        }
        
        protected override void ProcessChildNode(NodeRecord parentNode, NodeRecord childNode) {
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

    

        // Checks is if node(x,Y) is in the node(startx, starty) bounding box for the direction: direction
        public override bool InsindeGoalBoundBox(int startX, int startY, int x, int y, string direction)
        {
            if (!this.goalBounds.ContainsKey(new Vector2(startX, startY)))
                return false;

            if (!this.goalBounds[new Vector2(startX, startY)].ContainsKey(direction))
                return false;

            var box = this.goalBounds[new Vector2(startX, startY)][direction];
            
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
        
        protected override List<NodeRecord> GetNeighbourList(NodeRecord currentNode)
        {
            List<NodeRecord> neighbourList = new List<NodeRecord>();
            
            if (currentNode.x - 1 >= 0)
            {
                //WEST
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, "WEST"))
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            }
            if (currentNode.x + 1 < grid.getWidth())
            {
                //EAST
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, "EAST"))
                    neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            }
            if (currentNode.y - 1 >= 0)
            {
                //SOUTH
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, "SOUTH"))
                    neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
            }
            if (currentNode.y + 1 < grid.getHeight())
            {
                //NORTH
                if (InsindeGoalBoundBox(currentNode.x, currentNode.y, GoalNode.x, GoalNode.y, "NORTH"))
                    neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
            }
            
            return neighbourList;
        }
    }
}
