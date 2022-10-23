using System;
using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Formations;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static class GameConstants
    {
        public const float UPDATE_INTERVAL = 2.0f;
        public const int TIME_LIMIT = 200;
        public const int PICKUP_RANGE = 16;

    }
    //public fields, seen by Unity in Editor

    public AutonomousCharacter Character;
    public GameObject AnchorPrefab;
    public GameObject SlotPrefab;

    [Header("UI Objects")]
    public Text HPText;
    public Text ShieldHPText;
    public Text ManaText;
    public Text TimeText;
    public Text XPText;
    public Text LevelText;
    public Text MoneyText;
    public Text DiaryText;
    public GameObject GameEnd;

    [Header("Enemy Settings")]
    public bool StochasticWorld;
    public bool SleepingNPCs;
    public bool BehaviourTreeNPCs;
    public bool LineFormation;
    public bool TriangularFormation;

    //fields
    public List<GameObject> chests { get; set; }
    public List<GameObject> skeletons { get; set; }
    public List<GameObject> orcs { get; set; }
    public List<GameObject> dragons { get; set; }
    public List<GameObject> enemies { get; set; }
    public List<GameObject> patrols { get; set; }
    public Dictionary<string, List<GameObject>> disposableObjects { get; set; }
    public bool WorldChanged { get; set; }
    public List<Monster> MonstersInFormation { get; set; }
    public FormationManager FormationManager;

    private float nextUpdateTime = 0.0f;
    private float enemyAttackCooldown = 0.0f;
    public bool gameEnded { get; set; } = false;
    public Vector3 initialPosition { get; set; }

    void Awake()
    {
        Instance = this;
        UpdateDisposableObjects();
        this.WorldChanged = false;
        this.Character = GameObject.FindGameObjectWithTag("Player").GetComponent<AutonomousCharacter>();

        this.initialPosition = this.Character.gameObject.transform.position;
        
        // Forcing this class to be loaded
        PropertyKeys.Load();
    }

    public void UpdateDisposableObjects()
    {
        this.enemies = new List<GameObject>();
        this.disposableObjects = new Dictionary<string, List<GameObject>>();
        this.chests = GameObject.FindGameObjectsWithTag("Chest").ToList();
        this.skeletons = GameObject.FindGameObjectsWithTag("Skeleton").ToList();
        this.orcs = GameObject.FindGameObjectsWithTag("Orc").ToList();
        this.dragons = GameObject.FindGameObjectsWithTag("Dragon").ToList();
        this.patrols = GameObject.FindGameObjectsWithTag("PatrolPoint").ToList();
        this.enemies.AddRange(this.skeletons);
        this.enemies.AddRange(this.orcs);
        this.enemies.AddRange(this.dragons);

        MonstersInFormation = new List<Monster>();
     
        //adds all enemies to the disposable objects collection
        foreach (var enemy in this.enemies)
        {
            if (enemy.name.Contains("Formation")) {
                Monster monster = enemy.GetComponent<Monster>();
                    
                MonstersInFormation.Add(monster);
            }

            if (disposableObjects.ContainsKey(enemy.name))
            {
                this.disposableObjects[enemy.name].Add(enemy);
            }
            else this.disposableObjects.Add(enemy.name, new List<GameObject>() { enemy });
        }
        //add all chests to the disposable objects collection
        foreach (var chest in this.chests)
        {
            if (disposableObjects.ContainsKey(chest.name))
            {
                this.disposableObjects[chest.name].Add(chest);
            }
            else this.disposableObjects.Add(chest.name, new List<GameObject>() { chest });
        }
        //adds all health potions to the disposable objects collection
        foreach (var potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
        {
            if (disposableObjects.ContainsKey(potion.name))
            {
                this.disposableObjects[potion.name].Add(potion);
            }
            else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
        }
        //adds all mana potions to the disposable objects collection
        foreach (var potion in GameObject.FindGameObjectsWithTag("ManaPotion"))
        {
            if (disposableObjects.ContainsKey(potion.name))
            {
                this.disposableObjects[potion.name].Add(potion);
            }
            else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
        }
        
        // Dealing with orcs in formation
        if (MonstersInFormation.Count != 0) {
            // There are orcs who will take up a formation

            Vector3 position = new Vector3();
            FormationPattern pattern;
            
            // Choosing a pattern
            if (LineFormation) {
                pattern = new LineFormation();
            }
            else if (TriangularFormation) {
                pattern = new TriangularFormation();
            }
            else {
                pattern = new LineFormation();
            }
            
            foreach(Monster monster in MonstersInFormation) {
                position += monster.transform.position;
            }
            
            // Averaging the position
            position /= MonstersInFormation.Count;

            FormationManager = new FormationManager(MonstersInFormation, pattern, position, new Vector3(0, 0, -1));
            
            foreach(Monster monster in MonstersInFormation) {
                FormationManager.AddCharacter(monster);
            }
        }
    }

    void FixedUpdate()
    {
        if (!this.gameEnded)
        {

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + GameConstants.UPDATE_INTERVAL;
                this.Character.baseStats.Time += GameConstants.UPDATE_INTERVAL;
            }


            this.HPText.text = "HP: " + this.Character.baseStats.HP;
            this.XPText.text = "XP: " + this.Character.baseStats.XP;
            this.ShieldHPText.text = "Shield HP: " + this.Character.baseStats.ShieldHP;
            this.LevelText.text = "Level: " + this.Character.baseStats.Level;
            this.TimeText.text = "Time: " + this.Character.baseStats.Time;
            this.ManaText.text = "Mana: " + this.Character.baseStats.Mana;
            this.MoneyText.text = "Money: " + this.Character.baseStats.Money;

            if (this.Character.baseStats.HP <= 0 || this.Character.baseStats.Time >= GameConstants.TIME_LIMIT)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "You Died";
            }
            else if (this.Character.baseStats.Money >= 25)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "Victory \n GG EZ";
            }

            if (FormationManager != null) {
                FormationManager.UpdateSlots();
            }
        }
    }

    public void SwordAttack(GameObject enemy)
    {
        int damage = 0;

        Monster.EnemyStats enemyData = enemy.GetComponent<Monster>().enemyStats;

        if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
        {
            this.Character.AddToDiary(" I Sword Attacked " + enemy.name);

            if (this.StochasticWorld)
            {
                damage = enemy.GetComponent<Monster>().DmgRoll.Invoke();

                //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                int attackRoll = RandomHelper.RollD20() + 7;

                if (attackRoll >= enemyData.AC)
                {
                    //there was an hit, enemy is destroyed, gain xp
                    this.enemies.Remove(enemy);
                    this.disposableObjects[enemy.name].Remove(enemy);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }
            }
            else
            {
                damage = enemyData.SimpleDamage;
                this.enemies.Remove(enemy);
                this.disposableObjects[enemy.name].Remove(enemy);
                enemy.SetActive(false);
                Object.Destroy(enemy);
            }

            this.Character.baseStats.XP += enemyData.XPvalue;

            int remainingDamage = damage - this.Character.baseStats.ShieldHP;
            this.Character.baseStats.ShieldHP = Mathf.Max(0, this.Character.baseStats.ShieldHP - damage);

            if (remainingDamage > 0)
            {
                this.Character.baseStats.HP -= remainingDamage;
            }

            this.WorldChanged = true;
        }
    }
    
    public void DivineSmite(GameObject enemy)
    {
        Monster.EnemyStats enemyData = enemy.GetComponent<Monster>().enemyStats;

        if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
        {
            this.Character.AddToDiary(" I Divine Smited " + enemy.name);
            
            this.enemies.Remove(enemy);
            this.disposableObjects[enemy.name].Remove(enemy);
            enemy.SetActive(false);
            Object.Destroy(enemy);

            this.Character.baseStats.XP += enemyData.XPvalue;
            this.Character.baseStats.Mana -= 2;

            this.WorldChanged = true;
        }
    }

    public void EnemyAttack(GameObject enemy)
    {
        if (Time.time > this.enemyAttackCooldown)
        {

            int damage = 0;

            Monster monster = enemy.GetComponent<Monster>();

            if (enemy != null && enemy.activeSelf)
            {

                this.Character.AddToDiary(" I was Attacked by " + enemy.name);
                this.enemyAttackCooldown = Time.time + GameConstants.UPDATE_INTERVAL;

                if (this.StochasticWorld)
                {
                    damage = monster.DmgRoll.Invoke();

                    //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                    int attackRoll = RandomHelper.RollD20() + 7;

                    if (attackRoll >= monster.enemyStats.AC)
                    {
                        //there was an hit, enemy is destroyed, gain xp
                        this.enemies.Remove(enemy);
                        this.disposableObjects.Remove(enemy.name);
                        enemy.SetActive(false);
                        Object.Destroy(enemy);
                    }
                }
                else
                {
                    damage = monster.enemyStats.SimpleDamage;
                    this.enemies.Remove(enemy);
                    this.disposableObjects.Remove(enemy.name);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }

                this.Character.baseStats.XP += monster.enemyStats.XPvalue;

                int remainingDamage = damage - this.Character.baseStats.ShieldHP;
                this.Character.baseStats.ShieldHP = Mathf.Max(0, this.Character.baseStats.ShieldHP - damage);

                if (remainingDamage > 0)
                {
                    this.Character.baseStats.HP -= remainingDamage;
                    this.Character.AddToDiary(" I was wounded with " + remainingDamage + " damage");
                }

                this.WorldChanged = true;
            }
        }
    }

  
    public void PickUpChest(GameObject chest)
    {

        if (chest != null && chest.activeSelf && InChestRange(chest))
        {
            this.Character.AddToDiary(" I opened  " + chest.name);
            this.chests.Remove(chest);
            this.disposableObjects[chest.name].Remove(chest);
            Object.Destroy(chest);
            this.Character.baseStats.Money += 5;
            this.WorldChanged = true;
        }
    }


    public void GetHealthPotion(GameObject potion)
    {
        if (potion != null && potion.activeSelf && InPotionRange(potion))
        {
            this.Character.AddToDiary(" I drank " + potion.name);
            this.disposableObjects[potion.name].Remove(potion);
            Object.Destroy(potion);
            this.Character.baseStats.HP = this.Character.baseStats.MaxHP;
            this.WorldChanged = true;
        }
    }
    
    public void GetManaPotion(GameObject potion)
    {
        if (potion != null && potion.activeSelf && InPotionRange(potion))
        {
            this.Character.AddToDiary(" I drank " + potion.name);
            this.disposableObjects[potion.name].Remove(potion);
            Object.Destroy(potion);
            this.Character.baseStats.Mana = 10;
            this.WorldChanged = true;
        }
    }
    
    public void ShieldOfFaith()
    {
        this.Character.baseStats.ShieldHP = 5;
        this.Character.baseStats.Mana -= 5;
        this.WorldChanged = true;
    }

    public void LevelUp()
    {
        if (this.Character.baseStats.Level >= 4) return;

        if (this.Character.baseStats.XP >= this.Character.baseStats.Level * 10)
        {
            this.Character.baseStats.Level++;
            this.Character.baseStats.MaxHP += 10;
            this.Character.baseStats.XP = 0;
            this.WorldChanged = true;
            this.Character.AddToDiary(" I leveled up to level " + this.Character.baseStats.Level);
        }
    }

    public void Pray()
    {
        this.Character.baseStats.HP += AutonomousCharacter.REST_HP_RECOVERY;
        if (this.Character.baseStats.HP > this.Character.baseStats.MaxHP)
        {
            this.Character.baseStats.HP = this.Character.baseStats.MaxHP;
        }
    }

    public void SpeedUp()
    {
        this.Character.baseStats.Mana -= 5;
        this.Character.Speed *= AutonomousCharacter.SPEEDUP_ENHANCE;
        this.Character.navMeshAgent.speed *= AutonomousCharacter.SPEEDUP_ENHANCE;
    }

    private bool CheckRange(GameObject obj, float maximumSqrDistance)
    {
        var distance = (obj.transform.position - this.Character.gameObject.transform.position).sqrMagnitude;
        return distance <= maximumSqrDistance;
    }


    public bool InMeleeRange(GameObject enemy)
    {
        return this.CheckRange(enemy, GameConstants.PICKUP_RANGE);
    }

    public bool InChestRange(GameObject chest)
    {

        return this.CheckRange(chest, GameConstants.PICKUP_RANGE);
    }

    public bool InPotionRange(GameObject potion)
    {
        return this.CheckRange(potion, GameConstants.PICKUP_RANGE);
    }

}
