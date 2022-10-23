using System.Collections;
using System.Linq;
using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.IAJ.Unity.Formations
{

    public class TriangularFormation : FormationPattern 
    {
        private static readonly float offset = 1.0f;

        public TriangularFormation() {

        }

        public override Vector3 GetOrientation(FormationManager formation, Vector3 currentOrientation) {
            return formation.Anchor.transform.forward;
        }

        public override Vector3 GetSlotLocation(FormationManager formation, int slotNumber) {
            Vector3 orientation = GetOrientation(formation, new Vector3(0, 0, 0)).normalized;

            return formation.AnchorPosition + offset * (Quaternion.AngleAxis(120 * slotNumber, Vector3.up) * orientation);
        }

        public override  bool SupportsSlot(int slotCount)
        {
            return (slotCount <= 3); 
        }

    
    }
}