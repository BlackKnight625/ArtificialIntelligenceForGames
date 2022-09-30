
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public enum NodeStatus
    {
        Unvisited,
        Open,
        Closed
    }

    public class NodeRecord  : IComparable<NodeRecord>
    {
        //Coordinates
        public int x;
        public int y;
        public bool isWalkable;

        //A* Stuff
        public NodeRecord parent;
        public NodeRecord direction;
        public float gCost;
        public float hCost;
        public float fCost;

        // Node Record Array Index
        public int index;
        public NodeStatus status;
        
        public override string ToString()
        {
            return x + ":" + y;
        }

        public NodeRecord(int x, int y)
        {
            
            this.x = x;
            this.y = y;
            gCost = int.MaxValue;
            hCost = 0;
            fCost = gCost + hCost;
            parent = null;
            direction = null;
            index = 0;
            isWalkable = true;
            status = NodeStatus.Unvisited;

        }

        public NodeRecord(int x, int y, int _index) : this(x,y)
        {
            index = _index;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public int CompareTo(NodeRecord other)
        {
            return this.fCost.CompareTo(other.fCost);

        }

        //two node records are equal if they refer to the same node
        public override bool Equals(object obj)
        {
            if (obj is NodeRecord target) return this.x == target.x && this.y == target.y;
            else return false;
        }


        // I wonder where this might be useful...
        public void Reset()
        {
            gCost = int.MaxValue;
            hCost = 0;
            fCost = gCost + hCost;
            parent = null;
            direction = null;
            status = NodeStatus.Unvisited;
        }

    }
}
