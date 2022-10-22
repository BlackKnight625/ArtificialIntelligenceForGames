using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS;
using Assets.Scripts.Game;

public class AutonomousCharacter : NPC
{
    //constants
    public const string SURVIVE_GOAL = "Survive";
    public const string GAIN_LEVEL_GOAL = "GainXP";
    public const string BE_QUICK_GOAL = "BeQuick";
    public const string GET_RICH_GOAL = "GetRich";
    public const float DECISION_MAKING_INTERVAL = 20.0f;
    public const int RESTING_INTERVAL_TICKS = 5 * 50;
    public const int REST_HP_RECOVERY = 2;
    public const int SPEEDUP_INTERVAL_TICKS = 10 * 50;
    public const float SPEEDUP_ENHANCE = 2.0f;

    //UI Variables
    private Text SurviveGoalText;
    private Text GainXPGoalText;
    private Text BeQuickGoalText;
    private Text GetRichGoalText;
    private Text DiscontentmentText;
    private Text TotalProcessingTimeText;
    private Text BestDiscontentmentText;
    private Text ProcessedActionsText;
    private Text BestActionText;
    private Text BestActionSequence;
    private Text DiaryText;

    [Header("Character Settings")]
    public bool controlledByPlayer;
    public float Speed = 5.0f;

    [Header("Decision Algorithm Options")]
    public bool GOBActive;
    public bool GOAPActive;
    public bool MCTSActive;

    [Header("MCTS Playout action chooser")]
    public bool MCTSPlayoutRandomAction;
    public bool MCTSPlayoutBiasedAction;

    [Header("Character Info")]
    public bool Resting = false;
    public float StopRestTime;

    public Goal BeQuickGoal { get; private set; }
    public Goal SurviveGoal { get; private set; }
    public Goal GetRichGoal { get; private set; }
    public Goal GainLevelGoal { get; private set; }
    public List<Goal> Goals { get; set; }
    public List<Action> Actions { get; set; }
    public Action CurrentAction { get; private set; }
    public GOBDecisionMaking GOBDecisionMaking { get; set; }
    public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }
    public MCTS MCTS { get; set; }

    //private fields for internal use only

    private float nextUpdateTime = 0.0f;
    private float previousGold = 0.0f;
    private int previousLevel = 1;
    public TextMesh playerText;
    private GameObject closestObject;
    private AudioSource _praySound;
    private AudioSource _speedUpSound;

    // Draw path settings
    private LineRenderer lineRenderer;

    public void Initialize()
    {
    }

    public void Start() {
        _praySound = GetComponents<AudioSource>().FirstOrDefault(a => a.clip.name == "aaahmmmmm");
        _speedUpSound = GetComponents<AudioSource>().FirstOrDefault(a => a.clip.name == "gotta-go-fast");
        
        //This is the actual speed of the agent
        lineRenderer = this.GetComponent<LineRenderer>();
        playerText.text = "";

        this.Initialize();

        // Initializing UI Text
        this.BeQuickGoalText = GameObject.Find("BeQuickGoal").GetComponent<Text>();
        this.SurviveGoalText = GameObject.Find("SurviveGoal").GetComponent<Text>();
        this.GainXPGoalText = GameObject.Find("GainXP").GetComponent<Text>();
        this.GetRichGoalText = GameObject.Find("GetRichGoal").GetComponent<Text>();
        this.DiscontentmentText = GameObject.Find("Discontentment").GetComponent<Text>();
        this.TotalProcessingTimeText = GameObject.Find("ProcessTime").GetComponent<Text>();
        this.BestDiscontentmentText = GameObject.Find("BestDicont").GetComponent<Text>();
        this.ProcessedActionsText = GameObject.Find("ProcComb").GetComponent<Text>();
        this.BestActionText = GameObject.Find("BestAction").GetComponent<Text>();
        this.BestActionSequence = GameObject.Find("BestActionSequence").GetComponent<Text>();
        DiaryText = GameObject.Find("DiaryText").GetComponent<Text>();


        //initialization of the GOB decision making
        //let's start by creating 4 main goals

        this.SurviveGoal = new Goal(SURVIVE_GOAL, 1.0f);

        this.GainLevelGoal = new Goal(GAIN_LEVEL_GOAL, 4.0f)
        {
            InsistenceValue = 1.0f,
            ChangeRate = 1.0f
        };

        this.GetRichGoal = new Goal(GET_RICH_GOAL, 0.5f)
        {
            InsistenceValue = 2.0f,
            ChangeRate = 0.5f
        };

        this.BeQuickGoal = new Goal(BE_QUICK_GOAL, 1.0f)
        {
            ChangeRate = 0.1f
        };

        this.Goals = new List<Goal>();
        this.Goals.Add(this.SurviveGoal);
        this.Goals.Add(this.BeQuickGoal);
        this.Goals.Add(this.GetRichGoal);
        this.Goals.Add(this.GainLevelGoal);

        //initialize the available actions
        //Uncomment commented actions after you implement them

        this.Actions = new List<Action>();

        this.Actions.Add(new LevelUp(this));
        
        this.Actions.Add(new ShieldOfFaith(this));
        
        this.Actions.Add(new Pray(this));
        
        this.Actions.Add(new SpeedUp(this));

        foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
        {
            this.Actions.Add(new PickUpChest(this, chest.GetComponent<Disposable>()));
        }

        foreach (var potion in GameObject.FindGameObjectsWithTag("ManaPotion"))
        {
            this.Actions.Add(new GetManaPotion(this, potion.GetComponent<Disposable>()));
        }

        foreach (var potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
        {
            this.Actions.Add(new GetHealthPotion(this, potion.GetComponent<Disposable>()));
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Skeleton"))
        {
            this.Actions.Add(new SwordAttack(this, enemy.GetComponent<Disposable>()));
            this.Actions.Add(new DivineSmite(this, enemy.GetComponent<Disposable>()));
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Orc"))
        {
            this.Actions.Add(new SwordAttack(this, enemy.GetComponent<Disposable>()));
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Dragon"))
        {
            this.Actions.Add(new SwordAttack(this, enemy.GetComponent<Disposable>()));
        }

        StartCoroutine(InitDecisionAlgorithms());
        
        this.Resting = false;

        DiaryText.text += "My Diary \n I awoke. What a wonderful day to kill Monsters! \n";
    }

    private IEnumerator InitDecisionAlgorithms() {
        // Initialization of Decision Making Algorithms
        MCTSPlayoutActionChooser mctsPlayoutActionChooser;
        
        if (MCTSPlayoutRandomAction) {
            mctsPlayoutActionChooser = new RandomActionChooser();
        }
        else if (MCTSPlayoutBiasedAction) {
            mctsPlayoutActionChooser = new BiasedActionChooser();
        }
        else {
            mctsPlayoutActionChooser = new RandomActionChooser();
        }
        
        var worldModel = new CurrentStateWorldModel(GameManager.Instance, this.Actions, this.Goals);
        this.GOBDecisionMaking = new GOBDecisionMaking(this.Actions, this.Goals);
        this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel, this.Actions, this.Goals);
        this.MCTS = new MCTS(worldModel, mctsPlayoutActionChooser);

        yield return null;
    } 

    void FixedUpdate()
    {
        if (GameManager.Instance.gameEnded) return;

        if (Time.time > this.nextUpdateTime || GameManager.Instance.WorldChanged)
        {


            GameManager.Instance.WorldChanged = false;
            this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

            //first step, perceptions
            //update the agent's goals based on the state of the world

            // Max Health minus current Health
            this.SurviveGoal.InsistenceValue = baseStats.MaxHP - baseStats.HP;
            // Normalize it to 0-10
            this.SurviveGoal.InsistenceValue = NormalizeGoalValues(this.SurviveGoal.InsistenceValue, 0, baseStats.Level * 10);

            // Is this the best way to increment it?
            this.BeQuickGoal.InsistenceValue = baseStats.Time;
            this.BeQuickGoal.InsistenceValue = NormalizeGoalValues(this.BeQuickGoal.InsistenceValue, 0, (float)GameManager.GameConstants.TIME_LIMIT);

            this.GainLevelGoal.InsistenceValue += this.GainLevelGoal.ChangeRate; //increase in goal over time
            if (baseStats.Level > this.previousLevel)
            {
                this.GainLevelGoal.InsistenceValue -= baseStats.Level - this.previousLevel;
                this.previousLevel = baseStats.Level;
            }
            this.GainLevelGoal.InsistenceValue = NormalizeGoalValues(this.GainLevelGoal.InsistenceValue, 0, baseStats.Level * 10);

            this.GetRichGoal.InsistenceValue += this.GetRichGoal.ChangeRate;
            if (baseStats.Money > this.previousGold)
            {
                this.GetRichGoal.InsistenceValue -= baseStats.Money - this.previousGold;
                this.previousGold = baseStats.Money;
            }
            // Is this the best way to increment it?
            this.GetRichGoal.InsistenceValue = NormalizeGoalValues(this.GetRichGoal.InsistenceValue, 0, 25);

            this.SurviveGoalText.text = "Survive: " + this.SurviveGoal.InsistenceValue;
            this.GainXPGoalText.text = "Gain Level: " + this.GainLevelGoal.InsistenceValue.ToString("F1");
            this.BeQuickGoalText.text = "Be Quick: " + this.BeQuickGoal.InsistenceValue.ToString("F1");
            this.GetRichGoalText.text = "GetRich: " + this.GetRichGoal.InsistenceValue.ToString("F1");
            this.DiscontentmentText.text = "Discontentment: " + this.CalculateDiscontentment().ToString("F1");

            //To have a new decision lets initialize Decision Making Proccess
            this.CurrentAction = null;
            if (GOAPActive)
                this.GOAPDecisionMaking.InitializeDecisionMakingProcess();
            else if (GOBActive)
                this.GOBDecisionMaking.InProgress = true;
            else if (MCTSActive)
            {
                this.MCTS.InitializeMCTSearch();
            }

        }

        if (this.controlledByPlayer)
        {
            //Using the old Input System
            if (Input.GetKey(KeyCode.W))
                this.transform.position += new Vector3(0.0f, 0.0f, 0.1f) * this.Speed;
            if (Input.GetKey(KeyCode.S))
                this.transform.position += new Vector3(0.0f, 0.0f, -0.1f) * this.Speed;
            if (Input.GetKey(KeyCode.A))
                this.transform.position += new Vector3(-0.1f, 0.0f, 0.0f) * this.Speed;
            if (Input.GetKey(KeyCode.D))
                this.transform.position += new Vector3(0.1f, 0.0f, 0.0f) * this.Speed;
            if (Input.GetKey(KeyCode.F))
                if (closestObject != null)
                {
                    //Simple way of checking which object is closest to Sir Uthgard
                    var s = playerText.text.ToString();
                    if (s.Contains("Potion"))
                        PickUpPotion(s);
                    else if (s.Contains("Chest"))
                        PickUpChest();
                    else if (s.Contains("Enemy"))
                        AttackEnemy();
                }
            if (Input.GetKey(KeyCode.L))
                GameManager.Instance.LevelUp();


        }
        else if (this.GOAPActive)
        {
            this.UpdateDLGOAP();
        }
        else if (this.GOBActive)
        {
            this.UpdateGOB();
        }
        else if (this.MCTSActive)
        {
            this.UpdateMCTS();
        }

        if (this.CurrentAction != null)
        {
            if (this.CurrentAction.CanExecute())
            {
                this.CurrentAction.Execute();
            }
        }
        if (navMeshAgent.hasPath)
        {
            DrawPath();
        }
    }

    public void AddToDiary(string s)
    {
        DiaryText.text += Time.time + s + "\n";

        if (DiaryText.text.Length > 600)
            DiaryText.text = DiaryText.text.Substring(500);
    }

   
    private void UpdateGOB()
    {

        bool newDecision = false;
        if (this.GOBDecisionMaking.InProgress)
        {
            //choose an action using the GOB Decision Making process
            var action = this.GOBDecisionMaking.ChooseAction();
            if (action != null && action != this.CurrentAction)
            {
                this.CurrentAction = action;
                newDecision = true;
                if (newDecision)
                {
                    AddToDiary(" I decided to " + action.Name);
                    this.BestActionText.text = "Best Action: " + action.Name + "\n";
                    this.BestActionSequence.text = " Second Best:" + this.GOBDecisionMaking.secondBestAction.Name + "\n";
                }

            }

        }

    }

    private void UpdateDLGOAP()
    {
        bool newDecision = false;
        if (this.GOAPDecisionMaking.InProgress)
        {
            //choose an action using the GOB Decision Making process
            var action = this.GOAPDecisionMaking.ChooseAction();
            if (action != null && action != this.CurrentAction)
            {
                this.CurrentAction = action;
                newDecision = true;
            }
        }

        this.TotalProcessingTimeText.text = "Process. Time: " + this.GOAPDecisionMaking.TotalProcessingTime.ToString("F");
        this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue.ToString("F");
        this.ProcessedActionsText.text = "Act. comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;

        if (this.GOAPDecisionMaking.BestAction != null)
        {
            if (newDecision)
            {
                AddToDiary(" I decided to " + GOAPDecisionMaking.BestAction.Name);
            }
            var actionText = "";
            foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
            {
                if (action != null) {
                    actionText += "\n" + action.Name;
                }
            }
            this.BestActionSequence.text = "Best Action Sequence: " + actionText;
            this.BestActionText.text = "Best Action: " + GOAPDecisionMaking.BestAction.Name;
        }
        else
        {
            this.BestActionSequence.text = "Best Action Sequence:\nNone";
            this.BestActionText.text = "Best Action: \n Node";
        }
    }
    
    private void UpdateMCTS()
    {
        bool newDecision = false;
        if (this.MCTS.InProgress)
        {
            //choose an action using the GOB Decision Making process
            var action = this.MCTS.Run();
            if (action != null && action != this.CurrentAction)
            {
                this.CurrentAction = action;
                newDecision = true;
            }
        }

        this.TotalProcessingTimeText.text = "Process. Time: " + this.MCTS.TotalProcessingTime.ToString("F");
        this.BestDiscontentmentText.text = "Current Iterations: " + this.MCTS.CurrentIterations;
        this.ProcessedActionsText.text = "Current Depth: " + this.MCTS.CurrentDepth;

        if (newDecision)
        {
            AddToDiary(" I decided to " + this.CurrentAction.Name);
        }
        var actionText = "";
        if (this.MCTS.BestActionSequence != null)
        {
            foreach (var action in this.MCTS.BestActionSequence)
            {
                actionText += "\n" + action.Name;
            }

            this.BestActionSequence.text = "Best Action Sequence: " + actionText;
            this.BestActionText.text = "Best Action: " + this.CurrentAction?.Name;
        }
    }

      void DrawPath()
    {
       
        lineRenderer.positionCount = navMeshAgent.path.corners.Length;
        lineRenderer.SetPosition(0, this.transform.position);

        if (navMeshAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < navMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(navMeshAgent.path.corners[i].x, navMeshAgent.path.corners[i].y, navMeshAgent.path.corners[i].z);
            lineRenderer.SetPosition(i, pointPosition);
        }

    }


    public float CalculateDiscontentment()
    {
        var discontentment = 0.0f;

        foreach (var goal in this.Goals)
        {
            discontentment += goal.GetDiscontentment();
        }
        return discontentment;
    }

    //Functions designed for when the Player has control of the character
    void OnTriggerEnter(Collider col)
    {
        if (this.controlledByPlayer)
        {
            if (col.gameObject.tag.ToString().Contains("Potion"))
            {
                playerText.text = "Pickup Potion";
                closestObject = col.gameObject;
            }
            else if (col.gameObject.tag.ToString().Contains("Chest"))
            {
                playerText.text = "Pickup Chest";
                closestObject = col.gameObject;
            }
            else if (col.gameObject.tag.ToString().Contains("Orc") || col.gameObject.tag.ToString().Contains("Skeleton") || col.gameObject.tag.ToString().Contains("Dragon"))
            {
                playerText.text = "Attack Enemy";
                closestObject = col.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag.ToString() != "")
            playerText.text = "";
    }


    //Functions designed for when the Player has control of the character
    void PickUpPotion(string type)
    {
        if (closestObject != null)
            if (GameManager.Instance.InPotionRange(closestObject))
            {
                if (type.Contains("Mana"))
                    Debug.Log("Trying to Pickup Mana but Method is not implemented"); // todo
                else
                    GameManager.Instance.GetHealthPotion(closestObject);
                closestObject = null;
                playerText.text = "";
            }
    }

    void PickUpChest()
    {
        if (closestObject != null)
            if (GameManager.Instance.InChestRange(closestObject))
            {
                GameManager.Instance.PickUpChest(closestObject);
                closestObject = null;
                playerText.text = "";
            }
    }

    void AttackEnemy()
    {
        if (closestObject != null)
            if (GameManager.Instance.InMeleeRange(closestObject))
            {
                GameManager.Instance.SwordAttack(closestObject);
                closestObject = null;
                playerText.text = "";
            }
    }


    // Normalize different goal values to 0-10 ranges according to their max
    float NormalizeGoalValues(float value, float min, float max)
    {
        if (value < 0) value = 0.0f;
        // Normalizing to 0-1
        var x = (value - min) / (max - min);

        // Multiplying it by 10
        x *= 10;

        return x;
    }

    public void playPraySound() {
        _praySound.Play();
    }
    
    public void playSpeedUpSound() {
        _speedUpSound.Play();
    }

}
