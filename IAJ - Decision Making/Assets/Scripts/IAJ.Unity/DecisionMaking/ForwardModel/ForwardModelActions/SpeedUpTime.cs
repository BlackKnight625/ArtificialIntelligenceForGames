using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class SpeedUpTime
    {

        public SpeedUpTime(AutonomousCharacter character)
        {
            character.StartCoroutine(SpeedUpTimeCoroutine(character));
        }

        IEnumerator SpeedUpTimeCoroutine(AutonomousCharacter character)
        {
            int ticks = 0;
            while (ticks < AutonomousCharacter.SPEEDUP_INTERVAL_TICKS)
            {
                yield return new WaitForFixedUpdate();
                ticks++;
            }
            character.Speed /= 2;
        }
    }
}