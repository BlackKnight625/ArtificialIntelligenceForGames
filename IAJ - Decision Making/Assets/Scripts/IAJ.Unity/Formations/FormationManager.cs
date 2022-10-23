using UnityEditor;
using UnityEngine;
using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Assets.Scripts.IAJ.Unity.Formations
{
    public class FormationManager
    {
        public Dictionary<Monster, int> SlotAssignment = new();

        private FormationPattern _pattern;

        // # A Static (i.e., position and orientation) representing the
        // # drift offset for the currently filled slots.

        public Anchor Anchor;
        public GameObject AnchorGameObject;
        public List<GameObject> AnchorSlotObjects = new List<GameObject>(); 
        public Vector3 AnchorPosition => Anchor.gameObject.transform.position;
        
        public Vector3 Orientation {
            get {
                Quaternion rotation = AnchorGameObject.transform.rotation;

                return new Vector3(rotation.x,rotation.y,rotation.z);
            }
            set => AnchorGameObject.transform.rotation = Quaternion.LookRotation(value);
        }

        public FormationManager(List<Monster> NPCs, FormationPattern pattern, Vector3 position, Vector3 orientation)
        {
            AnchorGameObject = GameObject.Instantiate(GameManager.Instance.AnchorPrefab, position, Quaternion.LookRotation(orientation));
            Anchor = AnchorGameObject.GetComponent<Anchor>();

            Anchor.FormationManager = this;

            _pattern = pattern;
            
            int i = 0;
            foreach (Monster npc in NPCs)
            {
                SlotAssignment[npc] = i;
                i++;
                
                GameObject slotObject = GameObject.Instantiate(GameManager.Instance.SlotPrefab, AnchorPosition, Anchor.transform.rotation);
                
                AnchorSlotObjects.Add(slotObject);
            }

            Anchor.GetComponent<NavMeshAgent>().Warp(position);
            
            // the pattern may define specific rules for the orientation...
            this.Orientation = pattern.GetOrientation( this, this.Orientation);
        }

        public void UpdateSlotAssignements()
        {
            int i = 0; 
            foreach(var npc in  SlotAssignment.Keys)
            {
                SlotAssignment[npc] = i;
                i++;
            }
        }

        public bool AddCharacter(Monster character)
        {
            var occupiedSlots = this.SlotAssignment.Count;
            if (_pattern.SupportsSlot(occupiedSlots + 1))
            {
                SlotAssignment.Add(character, occupiedSlots);

                GameObject slotObject = GameObject.Instantiate(GameManager.Instance.SlotPrefab, AnchorPosition, Anchor.transform.rotation);
                
                AnchorSlotObjects.Add(slotObject);
                
                this.UpdateSlotAssignements();
                
                return true;
            }
            else return false;
        }

        public void RemoveCharacter(Monster character)
        {
            SlotAssignment.Remove(character);

            if (SlotAssignment.Count != 0) {
                GameObject anchorSlot = AnchorSlotObjects[0];
                
                GameObject.Destroy(anchorSlot);
                AnchorSlotObjects.Remove(anchorSlot);
            }
            
            UpdateSlots();
        }

        public void UpdateSlots()
        {
            if (SlotAssignment.Count == 0) {
                return;
            }
            
            var anchor = AnchorPosition;


            foreach (var npc in SlotAssignment.Keys)
            {
                int slotNumber = SlotAssignment[npc];
                var slot = _pattern.GetSlotLocation(this, slotNumber);

                AnchorSlotObjects[slotNumber].transform.position = slot;
                AnchorSlotObjects[slotNumber].transform.rotation = Anchor.transform.rotation;
                
                npc.StartPathfinding(slot);
                npc.GetComponent<NavMeshAgent>().updateRotation = true;
            }
        }
    }
}