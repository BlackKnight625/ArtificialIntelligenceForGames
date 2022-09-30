using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class NodeRecordArray : IOpenSet, IClosedSet
    {
        private NodePriorityHeap Open { get; set; }
        private int closedCount = 0;
        private List<NodeRecord> nodes;

        public NodeRecordArray(List<NodeRecord> nodes) {
            this.nodes = nodes;
            this.Open = new NodePriorityHeap();
        }

        void IOpenSet.Initialize()
        {
            this.Open.Initialize();
        }

        void IClosedSet.Initialize() {
            closedCount = 0;
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            this.Open.AddToOpen(nodeRecord);
            nodeRecord.status = NodeStatus.Open;
        }

        public void AddToClosed(NodeRecord nodeRecord) {
            nodeRecord.status = NodeStatus.Closed;

            closedCount++;
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord) {
            return Open.SearchInOpen(nodeRecord);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            if (nodeRecord.status == NodeStatus.Closed) {
                return nodeRecord;
            }
            else {
                return null;
            }
        }

        public NodeRecord GetBestAndRemove()
        {
            return this.Open.GetBestAndRemove();
        }

        public NodeRecord PeekBest()
        {
            return this.Open.PeekBest();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.Open.Replace(nodeToBeReplaced, nodeToReplace);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.Open.RemoveFromOpen(nodeRecord);
            nodeRecord.status = NodeStatus.Unvisited;
        }

        public void RemoveFromClosed(NodeRecord nodeRecord) {
            nodeRecord.status = NodeStatus.Unvisited;

            closedCount--;
        }

        ICollection<NodeRecord> IOpenSet.All()
        {
            return this.Open.All();
        }
        
        ICollection<NodeRecord> IClosedSet.All()
        {
            return nodes.Where(node => node.status == NodeStatus.Closed).ToList();
        }

        public int CountOpen()
        {
            return this.Open.CountOpen();
        }

        public int CountClosed() {
            return closedCount;
        }
    }
}
