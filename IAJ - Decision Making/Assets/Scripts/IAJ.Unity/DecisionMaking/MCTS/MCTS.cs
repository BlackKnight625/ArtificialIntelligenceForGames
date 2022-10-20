using Assets.Scripts.Game;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System;
using System.Collections.Generic;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<Action> BestActionSequence { get; private set; }
        public WorldModel BestActionSequenceWorldState { get; private set; }
        public int CurrentIterations { get; set; }
        public int CurrentIterationsInFrame { get; set; }
        public int CurrentDepth { get; set; }
        private int playoutDepth { get; set; }
        private int playoutsNumber { get; set; }
        protected CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }

        private MCTSPlayoutActionChooser _mctsPlayoutActionChooser;
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel, MCTSPlayoutActionChooser mctsPlayoutActionChooser)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random();
            this.TotalProcessingTime = 0.0f;

            _mctsPlayoutActionChooser = mctsPlayoutActionChooser;
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();

            // create root node n0 for state s0
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<Action>();
            this.playoutDepth = 50;
            this.playoutsNumber = 25;
        }

        public Action Run()
        {
            MCTSNode rootNode = InitialNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;
            //while within computational budget
            while (CurrentIterations < MaxIterations)
            {
                //Selection + Expansion
                MCTSNode selectedNode = Selection(rootNode);
                //Playout
                reward = Playout(selectedNode.State);
                
                //Backpropagation
                Backpropagate(selectedNode, reward);
                CurrentIterations++;
            }

            // return best initial child
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            return BestFinalAction(rootNode);
        }

        // Selection and Expantion
        protected MCTSNode Selection(MCTSNode initialNode)
        {
            Action nextAction;
            MCTSNode currentNode = initialNode;
            while (!currentNode.State.IsTerminal())
            {
                //Expansion
                if (!currentNode.IsFullyExpanded())
                {
                    return Expand(currentNode);
                }
                else
                {
                    currentNode = BestUCTChild(currentNode);
                }
            }
            return currentNode;
        }

        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
            
            int depth = 0;
            float totalScore = 0.0f;
            for (int i = 0; i < this.playoutsNumber; i++)
            {
                WorldModel playoutState = initialPlayoutState.GenerateChildWorldModel();
                while (!playoutState.IsTerminal() && depth < this.playoutDepth)
                {
                    Action[] executableActions = playoutState.GetExecutableActions();
                    _mctsPlayoutActionChooser.chooseAction(executableActions, playoutState).ApplyActionEffects(playoutState);
                    playoutState.ResetExecutableActions(); // Must reset since the actions available are now different
                    playoutState.CalculateNextPlayer();
                    depth++;
                }
                totalScore += playoutState.GetScore();
            }
            
            return new Reward(totalScore/playoutsNumber, initialPlayoutState.GetNextPlayer());
        }

        protected virtual void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N += 1;
                node.Q += reward.Value;
                node = node.Parent;
            }
        }

        protected MCTSNode Expand(MCTSNode parent) {
            Action action = parent.State.GetNextExecutableAction();
            
            WorldModel newState = parent.State.GenerateChildWorldModel();
            action.ApplyActionEffects(newState);
            newState.CalculateNextPlayer();
            MCTSNode child = new MCTSNode(newState);
            child.Action = action;
            child.Parent = parent;
            parent.ChildNodes.Add(child);

            return child;
        }

        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            
            if (node.N == 0)
                return node.ChildNodes[0];

            MCTSNode bestNode = null;
            float value;
            float bestValue = float.NegativeInfinity;
            foreach (MCTSNode child in node.ChildNodes)
            {
                if (child.N == 0)
                {
                    value = float.PositiveInfinity;
                }
                else
                {
                    float u = child.Q / child.N;
                    value = (float)(u + C * Math.Sqrt(Math.Log(node.N) / child.N));
                }
                if (value > bestValue)
                {
                    bestNode = child;
                    bestValue = value;
                }
            }
            return bestNode;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        protected MCTSNode BestChild(MCTSNode node)
        {
            if (node.ChildNodes.Count == 0 || node == null || node.ChildNodes == null)
                return null;
            MCTSNode bestNode = node.ChildNodes[0];
            foreach (MCTSNode child in node.ChildNodes)
            {
                if (child.State.GetScore() > bestNode.State.GetScore())
                {
                    bestNode = child;
                }
            }
            return bestNode;
        }

        
        protected Action BestFinalAction(MCTSNode node)
        {
            var bestChild = this.BestChild(node);
            if (bestChild == null) return null;

            this.BestFirstChild = bestChild;

            //this is done for debugging proposes only
            this.BestActionSequence = new List<Action>();
            this.BestActionSequence.Add(bestChild.Action);
            node = bestChild;

            while(!node.State.IsTerminal())
            {
                bestChild = this.BestChild(node);
                if (bestChild == null) break;
                this.BestActionSequence.Add(bestChild.Action);
                node = bestChild;
                this.BestActionSequenceWorldState = node.State;
            }

            return this.BestFirstChild.Action;
        }

    }
}
