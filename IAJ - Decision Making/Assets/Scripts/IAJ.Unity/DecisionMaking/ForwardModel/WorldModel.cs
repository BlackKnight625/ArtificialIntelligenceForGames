using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Utils;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel
{
    public class WorldModel {

        private List<Action> _executableActions;
        private IEnumerator<Action> _executableActionEnumerator;
        
        private Dictionary<string, object> Properties { get; set; }
        private List<Action> Actions { get; set; }
        protected List<Action> ExecutableActions => _executableActions ??= FindExecutableActions();

        protected IEnumerator<Action> ExecutableActionEnumerator => _executableActionEnumerator ??= ExecutableActions.GetEnumerator();

        private Dictionary<string, float> GoalValues { get; set; } 

        protected WorldModel Parent { get; set; }
        
        public int ExecutableActionsSize => ExecutableActions.Count;

        public WorldModel(List<Action> actions)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = new List<Action>(actions);
            this.Actions.Shuffle();
        }

        public WorldModel(WorldModel parent)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = new List<Action>(parent.Actions);
            this.Actions.Shuffle();
            this.Parent = parent;
        }

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            if (this.Properties.ContainsKey(propertyName))
            {
                return this.Properties[propertyName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetProperty(propertyName);
            }
            else
            {
                return null;
            }
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            this.Properties[propertyName] = value;
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.GoalValues.ContainsKey(goalName))
            {
                return this.GoalValues[goalName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetGoalValue(goalName);
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            this.GoalValues[goalName] = limitedValue;
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextExecutableAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ExecutableActionEnumerator.MoveNext()) {
                action = this.ExecutableActionEnumerator.Current;
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return ExecutableActions.ToArray();
        }

        public virtual bool IsTerminal()
        {
            if((int) (this.GetProperty("HP")) <= 0)
            {
                return true;
            }
            if ((int)(this.GetProperty("MONEY")) > 25)
            {
                return true;
            }
            if ((int)(this.GetProperty("TIME")) >= 200)
            {
                return true;
            }
            return false;
        }
        

        public virtual float GetScore()
        {
            if ((int)(GetProperty(Game.Properties.HP)) <= 0)
            {
                return 0.0f;
            }
            if ((int)(GetProperty(Game.Properties.MONEY)) >= 25)
            {
                return 1.0f;
            }
            if ((float)(this.GetProperty(Game.Properties.TIME)) >= 200)
            {
                return 0.0f;
            }
            
            //A fazer decidir weights
            return 0.35f * (((float) (int)GetProperty(Game.Properties.HP)) / (int)GetProperty(Game.Properties.MAXHP))
                + 0.3f * ((int)GetProperty(Game.Properties.MONEY) / 25.0f)
                + 0.3f * (1 - ((float)GetProperty(Game.Properties.TIME) / 200.0f));
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }

        protected List<Action> FindExecutableActions() {
            return this.Actions.Where(a => a.CanExecute(this)).ToList();
        }

        public void ResetExecutableActions() {
            _executableActions = FindExecutableActions();
            _executableActionEnumerator = _executableActions.GetEnumerator();
        }
    }
}
