using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Formations
{
    public class LineFormation : FormationPattern 
    {
        private static readonly float offset = 2.0f;

        public LineFormation()
        {

        }

        public override Vector3 GetOrientation(FormationManager formation, Vector3 currentOrientation) {
            return formation.Anchor.transform.forward;
        }

        public override Vector3 GetSlotLocation(FormationManager formation, int slotNumber) => slotNumber switch
        {
            0 => formation.AnchorPosition,
            _ => formation.AnchorPosition + offset * -slotNumber * this.GetOrientation(formation, new Vector3(0,0,0)).normalized
        };

        public override  bool SupportsSlot(int slotCount)
        {
            return (slotCount <= 3); 
        }

        
    }
}