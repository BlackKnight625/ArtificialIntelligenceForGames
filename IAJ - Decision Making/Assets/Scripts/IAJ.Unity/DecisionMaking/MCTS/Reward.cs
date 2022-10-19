namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class Reward
    {
        public float Value { get; set; }
        public int PlayerID { get; set; }

        public Reward(float value, int playerID)
        {
            this.Value = value;
            this.PlayerID = playerID;
        }
    }
}
