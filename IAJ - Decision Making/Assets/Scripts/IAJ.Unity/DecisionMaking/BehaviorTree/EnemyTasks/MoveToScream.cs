using Assets.Scripts.Game.NPCs;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks {
    public class MoveToScream : MoveTo {
        public MoveToScream(Monster character, float _range) : base(character, Orc.ScreamLocation, _range) {}

        public override Result Run() {
            // Updating the scream location
            Target = Orc.ScreamLocation;

            return base.Run();
        }
    }
}