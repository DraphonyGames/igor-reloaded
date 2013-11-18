using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// definition of save game data
/// </summary>
public class GameData
{
    /// <summary>
    /// the igor
    /// </summary>
    public IgorData igor;

    /// <summary>
    /// what to do
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// Save the game.
        /// </summary>
        SAVE,
        /// <summary>
        /// Load the game.
        /// </summary>
        LOAD
    }

    /// <summary>
    /// scene name
    /// </summary>
    public string scene;

    /// <summary>
    /// all prefabs
    /// </summary>
    public List<ScenePrefab> prefabs;

    /// <summary>
    /// all chests
    /// </summary>
    public List<ChestData> chests;

    /// <summary>
    /// all enemies
    /// </summary>
    public List<EnemyPrefab> enemies;

    /// <summary>
    /// store boss data
    /// </summary>
    public BossData boss;

    /// <summary>
    /// store transformers
    /// </summary>
    public List<BossTransformerData> transformers;
    /// <summary>
    /// all cut scenes
    /// </summary>
    public List<CutscenePrefab> cutscenes;

    /// <summary>
    /// store Jeve data
    /// </summary>
    public CutscenePrefabJeveData jeveData;

    /// <summary>
    /// skill bar stuff
    /// </summary>
    public SkillBarData skillBarData;

    /// <summary>
    /// skill stuff
    /// </summary>
    public SkillData skillData;

    /// <summary>
    /// the difficulty of game
    /// </summary>
    public GameDifficultyData gameDifficulty;

    /// <summary>
    /// GameData constructor with operation
    /// operation.SAVE: use it if save game
    /// operation.LOAD: use it if load game
    /// </summary>
    /// <param name="op">SAVE/LOAD or not SAVE/LOAD</param>
    public GameData(Operation op)
    {
        if (op == Operation.SAVE)
        {
            scene = Application.loadedLevelName;
            igor = new IgorData(Operation.SAVE);
            prefabs = new List<ScenePrefab>();
            chests = new List<ChestData>();
            enemies = new List<EnemyPrefab>();
            cutscenes = new List<CutscenePrefab>();
            jeveData = new CutscenePrefabJeveData();
            skillBarData = new SkillBarData(Operation.SAVE);
            skillData = new SkillData(Skills.GetSkills());
            gameDifficulty = new GameDifficultyData(Game.currentDifficulty);
            boss = new BossData();
            transformers = new List<BossTransformerData>();

            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.tag == "Need Save")
                {
                    prefabs.Add(new ScenePrefab(obj));
                }
                if (obj.tag == "Unit Enemy")
                {
                    enemies.Add(new EnemyPrefab(obj));
                }
                if (obj.tag.Contains("Cutscene"))
                {
                    cutscenes.Add(new CutscenePrefab(obj));
                }
                if (obj.tag == "Need Save,Usable")
                {
                    if (obj.name == "Chest")
                    {
                        chests.Add(new ChestData(obj));
                    }
                    if (obj.name == "PushBoxCrate")
                    {
                        prefabs.Add(new ScenePrefab(obj));
                    }
                }
                if (obj.tag == "Boss")
                {
                    boss = new BossData(obj);
                }
                if (obj.tag == "Unit,Transformer")
                {
                    transformers.Add(new BossTransformerData(obj));
                }
                if (obj.tag == "Need Save,Unit")
                {
                    jeveData = new CutscenePrefabJeveData(obj);
                }
            }
        }
        else if (op == Operation.LOAD)
        {
        }
        else
        {
            Debug.LogWarning("invalid operation !");
        }

    }

    /// <summary>
    /// null constructor for GameData
    /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    public GameData()
    {
    }

    #region IgorData

    /// <summary>
    /// class that stores igor data
    /// </summary>
    public class IgorData
    {

        //igor1 data

        /// <summary>
        /// igor1 prefab position
        /// </summary>
        public Vector3 igor1Position;

        /// <summary>
        /// igor1 prefab rotation
        /// </summary>
        public Quaternion igor1Rotation;

        //igor position data,for save

        /// <summary>
        /// igor position
        /// </summary>
        public Vector3 igorPosition;

        /// <summary>
        /// igor rotation
        /// </summary>
        public Quaternion igorRotation;

        //igor data,for save

        /// <summary>
        /// igor level
        /// </summary>
        public int igorLevel;

        /// <summary>
        /// igor experience
        /// </summary>
        public int igorExperience;

        /// <summary>
        /// igor health points
        /// </summary>
        public float igorHealthPoints;

        /// <summary>
        /// igor max health point
        /// </summary>
        public float igorMaxHealthPoints;

        /// <summary>
        /// igor speed
        /// </summary>
        public float igorSpeed;

        /// <summary>
        /// igor mana
        /// </summary>
        public float igorMana;

        /// <summary>
        /// igor max mana
        /// </summary>
        public float igorMaxMana;

        /// <summary>
        /// igor health regeneration
        /// </summary>
        public float igorHealthRegeneration;

        /// <summary>
        /// igor mana regeneration
        /// </summary>
        public float igorManaRegeneration;

        /// <summary>
        /// igor skill points
        /// </summary>
        public int igorSkillpoints;

        /// <summary>
        /// igor damage over time effects
        /// </summary>
        public List<CommonEntity.DamageOverTime> igorDamageOverTimeEffects;

        //igor inventory data
        /// <summary>
        /// the InventoryRecover list for save igor inventory
        /// </summary>
        public List<InventoryRecover> igorInventory;

        /// <summary>
        /// read igor's inventory data
        /// </summary>
        /// <returns>InventoryRecover list</returns>
        private List<InventoryRecover> IgorInventoryReader()
        {
            List<InventoryRecover> list = new List<InventoryRecover>(Game.GetIgor().GetComponent<Inventory>().items.Count);
            foreach (Inventory.InventoryItem item in Game.GetIgor().GetComponent<Inventory>().items)
            {
                list.Add(new InventoryRecover(item.stackS, item.instance.GetDisplayName(), item));
            }
            return list;
        }

        /// <summary>
        /// definition the igor's inventory
        /// </summary>
        public class InventoryRecover
        {
            /****************************
             * 
             * if igor has new items in it inventory add new attribute to here
             */
            /// <summary>
            /// the item amount of one slot
            /// </summary>
            public int amount;
            /// <summary>
            /// the item name of one slot
            /// </summary>
            public string name;
            /// <summary>
            /// how many hp a repairKit can repair
            /// </summary>
            public float repairKitAmountToHeal;
            /// <summary>
            /// the constructor for InventoryRecover
            /// </summary>
            /// <param name="stacks">InventoryItem stacks</param>
            /// <param name="iInvItemName">InventoryItem name</param>
            /// <param name="item">InventoryItem instance</param>
            public InventoryRecover(int stacks, string iInvItemName, Inventory.InventoryItem item)
            {
                amount = stacks;
                name = iInvItemName;
                if (name == "Repair Kit")
                {
                    repairKitAmountToHeal = ((RepairKit.InvRepairKit)item.instance).amountToHeal;

                }
            }
            /// <summary>
            /// null constructor for InventoryRecover
            /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            /// </summary>
            public InventoryRecover()
            {
            }
        }

        /// <summary>
        /// null constructor for IgorData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public IgorData()
        {
        }

        /// <summary>
        /// IgorData constructor with operation
        /// operation.SAVE: use it if save game
        /// operation.LOAD: use it if load game
        /// </summary>
        /// <param name="op">SAVE/LOAD or not SAVE/LOAD</param>
        public IgorData(Operation op)
        {
            if (op == Operation.SAVE)
            {
                GameObject igor1obj = GameObject.FindGameObjectWithTag("Igor Main Object");

                igor1Position = igor1obj.transform.position;
                igor1Rotation = igor1obj.transform.rotation;

                igorPosition = Game.GetIgor().transform.position;
                igorRotation = Game.GetIgor().transform.rotation;

                igorLevel = Game.GetIgor().GetComponent<Igor>().level;
                igorSpeed = Game.GetIgor().GetComponent<Igor>().speed;
                igorExperience = Game.GetIgor().GetComponent<Igor>().experience;
                igorHealthPoints = Game.GetIgor().GetComponent<Igor>().healthPoints;
                igorMaxHealthPoints = Game.GetIgor().GetComponent<Igor>().maxHealthPoints;
                igorHealthRegeneration = Game.GetIgor().GetComponent<Igor>().healthRegeneration;
                igorManaRegeneration = Game.GetIgor().GetComponent<Igor>().manaRegeneration;
                igorSkillpoints = Game.GetIgor().GetComponent<Igor>().skillpoints;
                igorDamageOverTimeEffects = Game.GetIgor().GetComponent<Igor>().damageOverTimeEffects;

                //Inventory of Igor
                igorInventory = IgorInventoryReader();
            }
            else if (op == Operation.LOAD)
            {
            }
            else
            {
                Debug.LogError("invalid operation !");
            }
        }
    }

    #endregion

    #region SkillBarData

    /// <summary>
    /// structure to save skill bar
    /// </summary>
    public class SkillBarData
    {
        /// <summary>
        /// the SkillSlot list for saving SkillBar slot
        /// </summary>
        public List<SkillSlot> slotList;

        /// <summary>
        /// null constructor of SkillBarData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public SkillBarData()
        {
        }

        /// <summary>
        /// constructor of SkillBarData,using in save game
        /// </summary>
        /// <param name="op">operation : Operation.SAVE or Operation.LOAD </param>
        public SkillBarData(Operation op)
        {
            if (op == Operation.SAVE)
            {
                slotList = new List<SkillSlot>();
                for (int i = 0; i < SkillBar.GetInstance().items.Count; i++)
                {
                    if (SkillBar.GetInstance().items[i] == null)
                    {
                        slotList.Add(new SkillSlot(null, SkillBar.SkillBarItem.ItemType.INVALID, i));
                    }
                    else
                    {
                        slotList.Add(new SkillSlot(SkillBar.GetInstance().items[i].name, SkillBar.GetInstance().items[i].type, i));
                        //        slotList.Add(new SkillSlot(obj.GetComponent<SkillBar>().items[i].name, i));
                    }
                }
            }
        }

        /// <summary>
        /// saving of a skill slot
        /// </summary>
        public class SkillSlot
        {
            /// <summary>
            /// the index of SkillBar slot
            /// </summary>
            public int index;

            /// <summary>
            /// the name of SkillBar slot
            /// </summary>
            public string name;

            /// <summary>
            /// the type of SkillBar slot
            /// </summary>
            public SkillBar.SkillBarItem.ItemType type;

            /// <summary>
            /// null constructor of SkillSlot
            /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            /// </summary>
            public SkillSlot()
            {
            }

            /// <summary>
            /// the constructor of SkillSlot
            /// </summary>
            /// <param name="skillName">the name of slot</param>
            /// <param name="skillIndex">the index of slot</param>
            public SkillSlot(string skillName, int skillIndex)
            {
                name = skillName;
                index = skillIndex;
            }

            /// <summary>
            /// the constructor of SkillSlot
            /// </summary>
            /// <param name="skillName">the name of slot</param>
            /// <param name="skillType">the type of slot</param>
            /// <param name="skillIndex">the index of slot</param>
            public SkillSlot(string skillName, SkillBar.SkillBarItem.ItemType skillType, int skillIndex)
            {
                name = skillName;
                type = skillType;
                index = skillIndex;
            }
        }
    }

    #endregion

    #region SkillData

    /// <summary>
    /// class to save skill data
    /// </summary>
    public class SkillData
    {   
        /// <summary>
        /// SkillDataList to save skill data
        /// </summary>
        public List<SkillDataList> skillDataList;

        /// <summary>
        /// null constructor for SkillData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public SkillData()
        {
        }

        /// <summary>
        /// constructor of SkillData
        /// </summary>
        /// <param name="skills">Skills.SkillData for Skills class</param>
        public SkillData(List<Skills.SkillData> skills)
        {
            skillDataList = new List<SkillDataList>();
            foreach (Skills.SkillData sd in skills)
            {
                skillDataList.Add(new SkillDataList(sd.skill.GetName(), sd.level));
            }
        }

        /// <summary>
        /// saves skill data
        /// </summary>
        public class SkillDataList
        {   
            /// <summary>
            /// the name of skill
            /// </summary>
            public string skillName;

            /// <summary>
            /// the level of skill
            /// </summary>
            public int skillLevel;

            /// <summary>
            /// null constructor for SkillDataList
            /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            /// </summary>
            public SkillDataList()
            {
            }

            /// <summary>
            /// constructor for SkillDataList
            /// </summary>
            /// <param name="name">for skill</param>
            /// <param name="level">for level</param>
            public SkillDataList(string name, int level)
            {
                skillName = name;
                skillLevel = level;
            }

        }

    }

    #endregion

    #region ScenePrefab

    /// <summary>
    /// the class of prefab in scene without igor,enemies
    /// </summary>
    public class ScenePrefab
    {   
        // prefab information
        /// <summary>
        /// the position of prefab
        /// </summary>
        public Vector3 prefabPosition;

        /// <summary>
        /// the rotation of prefab
        /// </summary>
        public Quaternion prefabRotation;

        /// <summary>
        /// the name of prefab
        /// </summary>
        public string prefabName;

        /// <summary>
        /// attribute of prefab
        /// if the instance prefab has more attribute,please add in here
        /// </summary>
        public float prefabAttribute1;

        /// <summary>
        /// attribute of prefab
        /// if the instance prefab has more attribute,please add in here
        /// </summary>
        public float prefabAttribute2;

        /// <summary>
        /// null constructor of ScenePrefab
        /// -- Attention: the null constructor is very important for serialization!! --
        /// </summary>
        public ScenePrefab()
        {
        }

        /// <summary>
        /// the prefab object
        /// </summary>
        /// <param name="obj">game object that want to be saved</param>
        public ScenePrefab(GameObject obj)
        {
            prefabPosition = obj.transform.position;
            prefabRotation = obj.transform.rotation;
            prefabName = obj.name;
        }
    }

    #endregion

    #region ChestData

    /// <summary>
    /// class to save chest data
    /// </summary>
    public class ChestData
    {
        /// <summary>
        /// the position of chest
        /// </summary>
        public Vector3 chestPosition;

        /// <summary>
        /// the rotation of chest
        /// </summary>
        public Quaternion chestRotation;

        /// <summary>
        /// whether a chest is opened
        /// </summary>
        public bool isChestOpened;

        /// <summary>
        /// the item names that chest contain
        /// </summary>
        public List<string> ChestItemsName = new List<string>();

        /// <summary>
        /// the name of chest object
        /// </summary>
        public string chestName;

        /// <summary>
        /// null constructor for ChestData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public ChestData()
        {
        }

        /// <summary>
        /// constructor of ChestData
        /// </summary>
        /// <param name="obj">the chest object</param>
        public ChestData(GameObject obj)
        {
            chestPosition = obj.transform.position;
            chestRotation = obj.transform.rotation;
            isChestOpened = obj.GetComponent<Chest>().open;
            chestName = obj.name;
            foreach (GameObject item in obj.GetComponent<Chest>().items)
            {
                ChestItemsName.Add(item.name);
            }
        }
    }

    #endregion

    #region EnemyPrefab

    /// <summary>
    /// the class of enemy
    /// </summary>
    public class EnemyPrefab
    {
        /// <summary>
        /// the name of EnemyPrefab
        /// </summary>
        public string enemyName;

        /// <summary>
        /// the position of EnemyPrefab
        /// </summary>
        public Vector3 enemyPosition;

        /// <summary>
        /// the rotation of EnemyPrefab
        /// </summary>
        public Quaternion enemyRotation;

        /// <summary>
        /// the initial position of enemy
        /// </summary>
        public Vector3 enemyInitialPosition;

        /// <summary>
        /// the hp of EnemyPrefab
        /// </summary>
        public float enemyHealthPoints;

        /// <summary>
        /// the way point information of EnemyPrefab
        /// </summary>
        public Vector3[] enemyNavMeshTransformsPosition;

        /// <summary>
        /// the current way point of EnemyPrefab
        /// </summary>
        public int enemyCurrentWaypoint;

        /// <summary>
        /// the faction of EnemyPrefab
        /// </summary>
        public CommonEntity.Faction enemyCurrentFaction;

        /// <summary>
        /// null constructor for EnemyPrefab
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public EnemyPrefab()
        {
        }

        /// <summary>
        /// constructor of EnemyPrefab
        /// </summary>
        /// <param name="obj">EnemyPrefab object </param>
        public EnemyPrefab(GameObject obj)
        {
            enemyName = obj.name;
            enemyPosition = obj.transform.position;
            enemyRotation = obj.transform.rotation;
            enemyInitialPosition = obj.GetComponent<EnemyAI>().initialPosition;
            enemyCurrentFaction = obj.GetComponent<CommonEntity>().currentFaction;

            // if have new enemy, add in under
            if (enemyName == "Arm")
            {
                enemyHealthPoints = obj.GetComponent<ArmEnemy>().healthPoints;
                enemyNavMeshTransformsPosition = obj.GetComponent<ArmEnemy>().NavMeshTransformsPosition;
                enemyCurrentWaypoint = obj.GetComponent<ArmEnemy>().currentWaypoint;
            }
            else if (enemyName == "DiagnosisRobotPrefab")
            {
                enemyHealthPoints = obj.GetComponent<DiagnosisRobotEnemy>().healthPoints;
                enemyNavMeshTransformsPosition = obj.GetComponent<DiagnosisRobotEnemy>().NavMeshTransformsPosition;
                enemyCurrentWaypoint = obj.GetComponent<DiagnosisRobotEnemy>().currentWaypoint;
            }
            else if (enemyName == "FlyingEyePrefab")
            {
                enemyHealthPoints = obj.GetComponent<FlyingEyeEnemy>().healthPoints;
                enemyNavMeshTransformsPosition = obj.GetComponent<FlyingEyeEnemy>().NavMeshTransformsPosition;
                enemyCurrentWaypoint = obj.GetComponent<FlyingEyeEnemy>().currentWaypoint;
            }
            else if (enemyName == "LittleFightingRobotPrefab")
            {
                enemyHealthPoints = obj.GetComponent<LittleFightingRobot>().healthPoints;
                enemyNavMeshTransformsPosition = obj.GetComponent<LittleFightingRobot>().NavMeshTransformsPosition;
                enemyCurrentWaypoint = obj.GetComponent<LittleFightingRobot>().currentWaypoint;
            }
            else
            {
            }
        }
    }

    #endregion

    #region BossData

    /// <summary>
    /// store boss data
    /// </summary>
    public class BossData
    {   
        /// <summary>
        /// store boss position
        /// </summary>
        public Vector3 bossPosition;

        /// <summary>
        /// store boss rotation
        /// </summary>
        public Quaternion bossRotation;

        /// <summary>
        /// store boss HP
        /// </summary>
        public float bossHP;

        /// <summary>
        /// null constructor for BossData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public BossData()
        { 
        }

        /// <summary>
        /// constructor for BossData
        /// </summary>
        /// <param name="obj">boss object</param>
        public BossData(GameObject obj)
        {
            bossPosition = obj.transform.position;
            bossRotation = obj.transform.rotation;
            bossHP = obj.GetComponent<BossAI>().healthPoints;
        }

    }

    #region BossTransformerData
    /// <summary>
    /// store boss transformer data
    /// </summary>
    public class BossTransformerData
    {
        /// <summary>
        /// store transformer position
        /// </summary>
        public Vector3 transformerPosition;
        /// <summary>
        /// store transformer state
        /// </summary>
        public CommonEntity.EntityState transformerState;

        /// <summary>
        /// null constructor for BossTransformerData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public BossTransformerData()
        { 
        }
        /// <summary>
        /// constructor of BossTransformerData
        /// </summary>
        /// <param name="obj">transformer obj</param>
        public BossTransformerData(GameObject obj)
        {
            transformerPosition = obj.transform.position;
            transformerState = obj.GetComponent<Transformer>().currentState;
        }
    }
    #endregion

    #endregion

    #region CutscenePrefab

    /// <summary>
    /// save cutscene data 
    /// </summary>
    public class CutscenePrefab
    {
        /// <summary>
        /// the InitialPosition of CutscenePrefab
        /// </summary>
        public Vector3 cutscenePrefabInitialPosition;

        /// <summary>
        /// the CurrentPosition of CutscenePrefab
        /// </summary>
        public Vector3 cutscenePrefabCurrentPosition;

        /// <summary>
        /// whether the cutscene is ran
        /// </summary>
        public bool cutscenePrefabIsRunned;

        /// <summary>
        /// null constructor of CutscenePrefab
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public CutscenePrefab()
        {
        }

        /// <summary>
        /// constructor of CutscenePrefab
        /// </summary>
        /// <param name="obj">the object</param>
        public CutscenePrefab(GameObject obj)
        {
            cutscenePrefabCurrentPosition = obj.transform.position;
            cutscenePrefabInitialPosition = obj.GetComponent<MovementManager>().initialPosition;
            cutscenePrefabIsRunned = obj.GetComponent<MovementManager>().isRun;
        }
    }

    #endregion

    #region CutscenePrefabJeveData
    /// <summary>
    /// store cutscene Jeve data
    /// </summary>
    public class CutscenePrefabJeveData
    {
        /// <summary>
        /// store Jeve position
        /// </summary>
        public Vector3 jevePosition;
        /// <summary>
        /// store Jeve rotation
        /// </summary>
        public Quaternion jeveRotation;

        /// <summary>
        /// null constructor for GameDifficultyData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public CutscenePrefabJeveData()
        { 
        }

        /// <summary>
        /// constructor of GameDifficultyData
        /// </summary>
        /// <param name="obj">the Jeve object</param>
        public CutscenePrefabJeveData(GameObject obj)
        {
            jevePosition = obj.transform.position;
            jeveRotation = obj.transform.rotation;
        }
    }
    #endregion

    #region GameDifficultyData

    /// <summary>
    /// save currentDifficulty data
    /// </summary>
    public class GameDifficultyData
    {   
        /// <summary>
        /// currentDifficulty data
        /// </summary>
        public Game.Difficulty difficulty;
        /// <summary>
        /// null constructor for GameDifficultyData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public GameDifficultyData()
        { 
        }

        /// <summary>
        /// constructor of GameDifficultyData
        /// </summary>
        /// <param name="dif">Game.Difficulty type object</param>
        public GameDifficultyData(Game.Difficulty dif)
        {
            difficulty = dif;
        }
    }

    #endregion


}
