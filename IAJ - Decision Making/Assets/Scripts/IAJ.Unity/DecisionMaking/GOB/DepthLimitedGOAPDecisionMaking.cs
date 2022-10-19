using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Game;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 4;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;

            var startTime = Time.realtimeSinceStartup;

            float currentValue;
            Action[] actions = new Action[MAX_DEPTH];
            Action nextAction;
            bool modelHasActions = InitialWorldModel.ExecutableActionsSize != 0;
            
            while (CurrentDepth >= 0)
            {
                if (!modelHasActions || CurrentDepth >= MAX_DEPTH)
                {
                    TotalActionCombinationsProcessed++;
                    currentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);
                    if (currentValue < BestDiscontentmentValue)
                    {
                        BestDiscontentmentValue = currentValue;
                        BestAction = actions[0];

                        for (int i = 0; i < actions.Length; i++) {
                            BestActionSequence[i] = actions[i];
                        }
                    }
                    CurrentDepth -= 1;
                    continue;
                }

                modelHasActions = Models[CurrentDepth].ExecutableActionsSize != 0;

                processedActions++;
                nextAction = Models[CurrentDepth].GetNextExecutableAction();
                if (nextAction != null)
                {
                    Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                    try {
                        nextAction.ApplyActionEffects(Models[CurrentDepth + 1]);
                    }
                    catch (MissingReferenceException e) {
                        Debug.Log("Hello");
                    }
                    actions[CurrentDepth] = nextAction;
                    CurrentDepth += 1;
                }
                else if (modelHasActions) {
                    // Ran out of actions (instead of starting with 0 actions)
                    CurrentDepth -= 1;   
                }
            }
            
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;
            return this.BestAction;
        }
    }
}
