using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class PrayTime
    {
        
        public PrayTime(AutonomousCharacter character)
        {
            character.StartCoroutine(PrayTimeCoroutine(character));
        }

        IEnumerator PrayTimeCoroutine(AutonomousCharacter character)
        {
            int ticks = 0;
            while (ticks < AutonomousCharacter.RESTING_INTERVAL_TICKS)
            {
                yield return new WaitForFixedUpdate();
                ticks++;
            }
            character.Resting = false;
            GameManager.Instance.WorldChanged = true;
        }
    }
}