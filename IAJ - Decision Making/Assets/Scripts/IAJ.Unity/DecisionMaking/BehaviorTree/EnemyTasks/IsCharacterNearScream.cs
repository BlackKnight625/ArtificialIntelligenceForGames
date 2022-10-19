using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class IsCharacterNearScream : IsCharacterNearTarget {
        public IsCharacterNearScream(NPC character, float _range) : base(character, Orc.ScreamLocation, _range) {}

        public override Result Run() {
            Target = Orc.ScreamLocation;
            
            return base.Run();
        }
    }
}