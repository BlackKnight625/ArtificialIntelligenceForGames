using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    class ClosedDictionary : IClosedSet
    {

        //Tentative dictionary type structure, it is possible that there are better solutions...
        private Dictionary<Vector2, NodeRecord> Closed { get; set; }

        public ClosedDictionary()
        {
            this.Closed = new Dictionary<Vector2, NodeRecord>();
        }

        public void Initialize()
        {
           Closed.Clear();
        }


        public void AddToClosed(NodeRecord nodeRecord)
        {
            Closed.Add(GetKey(nodeRecord), nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord) {
            Closed.Remove(GetKey(nodeRecord));
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord) {
            if (Closed.TryGetValue(GetKey(nodeRecord), out var result)) {
                return result;
            }
            else {
                return null;
            }
        }

        public ICollection<NodeRecord> All() {
            return Closed.Values;
        }

        public Vector2 GetKey(NodeRecord record) {
            return new Vector2(record.x, record.y);
        }
    }

}

