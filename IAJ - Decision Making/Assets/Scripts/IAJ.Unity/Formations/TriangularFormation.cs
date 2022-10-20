using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Formations
{

    public class TriangularFormation : FormationPattern 
    {
        // This is a very simple line formation, with the anchor being the position of the character at index 0.
        private static readonly float offset = 16.0f;

        private static readonly float height = 32.0f;

        public TriangularFormation()
        {

        }

        public override Vector3 GetOrientation(FormationManager formation, Vector3 orientation )
        {
            //In this formation, the orientation is defined by the first character's transform rotation...

            //antigo
            //Quaternion rotation = formation.SlotAssignment[0].transform.rotation;

            //novo
            Quaternion rotation = formation.SlotAssignment.Keys.First().transform.rotation;
      
            //Vector2 orientation = new Vector2(rotation.x, rotation.z);
            //return orientation;
            
            return new Vector3(rotation.x,rotation.y,rotation.z);
        }

        public override Vector3 GetSlotLocation(FormationManager formation, int slotNumber) => slotNumber switch
        {
            
            0 => formation.AnchorPosition,
            1 => formation.AnchorPosition + height * this.GetOrientation(formation, new Vector3(1,0,1)),
            2 => formation.AnchorPosition + height * this.GetOrientation(formation, new Vector3(-1,0,1))
        };

        public override  bool SupportSlot(int slotCount)
        {
            return (slotCount <= 3); 
        }

    
    }
}