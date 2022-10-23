using System.Collections;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Formations
{
    public abstract class FormationPattern
    {
        public abstract Vector3 GetOrientation(FormationManager formation, Vector3 currentOrientation);

        public abstract Vector3 GetSlotLocation(FormationManager formation, int slotNumber);

        public abstract bool SupportsSlot(int slotCount);
    }
}