using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// loads games
/// </summary>
public class GameDataRecover 
{
    /// <summary>
    /// null constructor of GameDataRecover
    /// </summary>
    public GameDataRecover()
    {
    }

    /// <summary>
    /// constructor of GameDataRecover
    /// recover game state from a GameData object
    /// </summary>
    /// <param name="gd">the GameData object </param>
    public GameDataRecover(GameData gd)
    {
        Application.LoadLevel(gd.scene);
        IgorRecover(gd);
    }

    #region GameDifficultyDataRecover

    /// <summary>
    /// recover game difficulty data
    /// </summary>
    /// <param name="gd">GameData object </param>
    public void GameDifficultyDataRecover(GameData gd)
    {
        Game.currentDifficulty = gd.gameDifficulty.difficulty;
    }

    #endregion

    #region LoadScene

    /// <summary>
    /// load game scene
    /// </summary>
    /// <param name="sceneName">name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    #endregion

    #region SkillBarRecover

    /// <summary>
    /// recover SkillBar
    /// </summary>
    /// <param name="gd">the GameData object that recover from </param>
    public void SkillBarRecover(GameData gd)
    {
  //      GameObject obj = (GameObject)GameObject.FindGameObjectWithTag("SkillBar");
  //      SkillBar sbar = (SkillBar)obj.GetComponent<SkillBar>();

        foreach (GameData.SkillBarData.SkillSlot slot in gd.skillBarData.slotList)
        {

            if (slot.type == SkillBar.SkillBarItem.ItemType.SKILL)
            {
                SkillBar.GetInstance().AssignSlot(slot.index, slot.name, SkillBar.SkillBarItem.ItemType.SKILL);
            }
            else if (slot.type == SkillBar.SkillBarItem.ItemType.ITEM)
            {
                SkillBar.GetInstance().AssignSlot(slot.index, slot.name, SkillBar.SkillBarItem.ItemType.ITEM);
            }
            else
            {
                SkillBar.GetInstance().items[slot.index] = null;
            }
            
        }

    }

    #endregion

    #region SkillTreeRecover

    /// <summary>
    /// recover SkillTreeRecover
    /// </summary>
    /// <param name="gd">the GameData object that recover from </param>
    public void SkillTreeRecover(GameData gd)
    {
        if (gd.skillData.skillDataList == null)
        {
            return; 
        }
        foreach (GameData.SkillData.SkillDataList sd in gd.skillData.skillDataList)
        {
            Skills.GetSkill(sd.skillName).level = sd.skillLevel;
        }
        Skills.ResetSkillsData();
    }

    #endregion

    #region ScenePrefabRecover

    /// <summary>
    /// recover prefabs and enemies
    /// </summary>
    /// <param name="gd">the GameData object that recover from </param>
    public void ScenePrefabRecover(GameData gd)
    {
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
        {
            if ((obj.tag == "Need Save" || obj.tag.Contains("Need Save")) && obj.tag != "Need Save,Unit")
            {
                if (obj.name != "RepairKitPrefab")
                {
                    MonoBehaviour.Destroy(obj);
                }
            }
        }
        foreach (GameData.ScenePrefab sd in gd.prefabs)
        {
            sd.prefabName = sd.prefabName.Replace("(Clone)", "");
            if (sd.prefabName != "RepairKitPrefab")
            {
                GameObject sdObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(sd.prefabName), sd.prefabPosition, sd.prefabRotation);
                sdObj.name = sdObj.name.Replace("(Clone)", "");
            }
            else
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Need Save");
                bool isNew = true;
                foreach (GameObject obj in objs)
                {
                    if (obj.name == "RepairKitPrefab")
                    {
                        if (obj.transform.position == sd.prefabPosition)
                        {
                            isNew = false;
                        }
                    }
                }
                if (isNew)
                {
                    GameObject sdObj = (GameObject)MonoBehaviour.Instantiate(Resources.Load(sd.prefabName), sd.prefabPosition, sd.prefabRotation);
                    sdObj.name = sdObj.name.Replace("(Clone)", "");
                }
            }
            
            
        }
        foreach (GameData.ChestData cd in gd.chests)
        {
            cd.chestName = cd.chestName.Replace("(Clone)", "");
            GameObject chest = (GameObject)MonoBehaviour.Instantiate(Resources.Load(cd.chestName), cd.chestPosition, cd.chestRotation);
            chest.name = chest.name.Replace("(Clone)", "");
            chest.GetComponent<Chest>().open = cd.isChestOpened;
            if (!cd.isChestOpened)
            {
                foreach (string itemName in cd.ChestItemsName)
                {
                    GameObject item = (GameObject)MonoBehaviour.Instantiate(Resources.Load(itemName.Replace("(Clone)", "")), new Vector3(0, -100, 0), Quaternion.identity);
                    chest.GetComponent<Chest>().items.Add(item);
                }
            }
        }
        /*
            attention!!!!!!!!!!!!!!
        
         
            if new enemy be added, maybe need to add new code in here 
        */

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Unit Enemy");
        if (enemies != null)
        {
            foreach (GameObject enemy in enemies)
            {
                bool isDead = true;
                foreach (GameData.EnemyPrefab ep in gd.enemies)
                {
                    if (enemy.GetComponent<EnemyAI>().initialPosition == ep.enemyInitialPosition)
                    {
                        enemy.GetComponent<EnemyAI>().isloaded = true;
                        enemy.GetComponent<EnemyAI>().healthPoints = ep.enemyHealthPoints;
                        enemy.GetComponent<CommonEntity>().currentFaction = ep.enemyCurrentFaction;
                        enemy.GetComponent<EnemyAI>().currentWaypoint = ep.enemyCurrentWaypoint;
                        enemy.transform.position = ep.enemyPosition;
                        enemy.transform.rotation = ep.enemyRotation;
                        if (enemy.GetComponent<EnemyAI>().nma != null)
                        {
                            enemy.GetComponent<EnemyAI>().nma.ResetPath();
                            enemy.GetComponent<EnemyAI>().nma.Resume();
                        }
                        isDead = false;
                    }
                }
                if (isDead)
                {
                    MonoBehaviour.Destroy(enemy);
                }
            }
        }
        /*
        foreach (GameData.EnemyPrefab ep in gd.enemies)
        {
            ep.enemyName = ep.enemyName.Replace("(Clone)", "");

            GameObject enemy = (GameObject)MonoBehaviour.Instantiate(Resources.Load(ep.enemyName), ep.enemyPosition, ep.enemyRotation);
            enemy.name = enemy.name.Replace("(Clone)", "");
            enemy.GetComponent<CommonEntity>().currentFaction = ep.enemyCurrentFaction;
            enemy.GetComponent<EnemyAI>().isloaded = true;

            if (enemy.name == "Arm")
            {
                enemy.GetComponent<ArmEnemy>().healthPoints = ep.enemyHealthPoints;
                enemy.GetComponent<EnemyAI>().NavMeshTransformsPosition = ep.enemyNavMeshTransformsPosition;
                enemy.GetComponent<EnemyAI>().currentWaypoint = ep.enemyCurrentWaypoint;
            }
            else if (enemy.name == "DiagnosisRobotPrefab")
            {
                enemy.GetComponent<DiagnosisRobotEnemy>().healthPoints = ep.enemyHealthPoints;
                enemy.GetComponent<EnemyAI>().NavMeshTransformsPosition = ep.enemyNavMeshTransformsPosition;
                enemy.GetComponent<EnemyAI>().currentWaypoint = ep.enemyCurrentWaypoint;
            }
            else if (enemy.name == "FlyingEyePrefab")
            {
                enemy.GetComponent<FlyingEyeEnemy>().healthPoints = ep.enemyHealthPoints;
                enemy.GetComponent<EnemyAI>().NavMeshTransformsPosition = ep.enemyNavMeshTransformsPosition;
                enemy.GetComponent<EnemyAI>().currentWaypoint = ep.enemyCurrentWaypoint;
            }
            else if (enemy.name == "LittleFightingRobotPrefab")
            {
                enemy.GetComponent<LittleFightingRobot>().healthPoints = ep.enemyHealthPoints;
                enemy.GetComponent<EnemyAI>().NavMeshTransformsPosition = ep.enemyNavMeshTransformsPosition;
                enemy.GetComponent<EnemyAI>().currentWaypoint = ep.enemyCurrentWaypoint;
            }
            else
            {
            }
        }
         * */
    }

    #endregion

    #region BossDataRecover

    /// <summary>
    /// recover boss data
    /// </summary>
    /// <param name="gd">GameData object</param>
    public void BossDataRecover(GameData gd)
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            boss.transform.position = gd.boss.bossPosition;
            boss.transform.rotation = gd.boss.bossRotation;
            boss.GetComponent<BossAI>().healthPoints = gd.boss.bossHP;
        }
        GameObject[] transformers = GameObject.FindGameObjectsWithTag("Unit,Transformer");
        if (transformers != null)
        {
            foreach (GameData.BossTransformerData transformer in gd.transformers)
            {
                foreach (GameObject trans in transformers)
                {
                    if (trans.transform.position == transformer.transformerPosition)
                    {
                        trans.GetComponent<Transformer>().currentState = transformer.transformerState;
                        if (transformer.transformerState == CommonEntity.EntityState.dead)
                        {
                            if (boss != null)
                            {
                                boss.GetComponent<BossAI>().transformer.Remove(trans);
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region CutscenePrefabRecover

    /// <summary>
    /// recover cutscene information
    /// </summary>
    /// <param name="gd">the GameData object that recover from </param>
    public void CutscenePrefabRecover(GameData gd)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Cutscene");
        foreach (GameData.CutscenePrefab cp in gd.cutscenes)
        {
            foreach (GameObject obj in objs)
            {
                if (obj.transform.position == cp.cutscenePrefabInitialPosition)
                {
                    Debug.Log(obj.name);
                    obj.GetComponent<MovementManager>().isRun = cp.cutscenePrefabIsRunned;
                    obj.transform.position = cp.cutscenePrefabCurrentPosition;
                }
            }
        }

        GameObject[] objs2 = GameObject.FindGameObjectsWithTag("Cutscene,Unit");
        foreach (GameData.CutscenePrefab cp in gd.cutscenes)
        {
            foreach (GameObject obj in objs2)
            {
                if (obj.transform.position == cp.cutscenePrefabInitialPosition)
                {
                    obj.GetComponent<MovementManager>().isRun = cp.cutscenePrefabIsRunned;
                    obj.transform.position = cp.cutscenePrefabCurrentPosition;
                }
            }
        }

        GameObject jeveObject = GameObject.FindGameObjectWithTag("Need Save,Unit");
        if (jeveObject != null)
        {
            jeveObject.transform.position = gd.jeveData.jevePosition;
            jeveObject.transform.rotation = gd.jeveData.jeveRotation;
        }
    }

    #endregion

    #region IgorRecover

    /// <summary>
    /// recover igor data from GameDataS
    /// </summary>
    /// <param name="gd"> the GameData used to recover </param>
    public void IgorRecover(GameData gd)
    {
        if (Game.GetIgor() == null)
        {
       //     MonoBehaviour.DontDestroyOnLoad(MonoBehaviour.Instantiate(Resources.Load("IGOR 1"), gd.igor.igor1Position, gd.igor.igor1Rotation));
            MonoBehaviour.Instantiate(Resources.Load("IGOR 1"), gd.igor.igor1Position, gd.igor.igor1Rotation);
        }

        Game.GetIgor().transform.position = gd.igor.igorPosition;
        Game.GetIgor().transform.rotation = gd.igor.igorRotation;


        Game.GetIgor().GetComponent<Igor>().level = gd.igor.igorLevel;
        Game.GetIgor().GetComponent<Igor>().speed = gd.igor.igorSpeed;
        Game.GetIgor().GetComponent<Igor>().experience = gd.igor.igorExperience;
        Game.GetIgor().GetComponent<Igor>().healthPoints = gd.igor.igorHealthPoints;
        Game.GetIgor().GetComponent<Igor>().maxHealthPoints = gd.igor.igorMaxHealthPoints;
        Game.GetIgor().GetComponent<Igor>().healthRegeneration = gd.igor.igorHealthRegeneration;
        Game.GetIgor().GetComponent<Igor>().manaRegeneration = gd.igor.igorManaRegeneration;
        Game.GetIgor().GetComponent<Igor>().skillpoints = gd.igor.igorSkillpoints;
        Game.GetIgor().GetComponent<Igor>().damageOverTimeEffects = gd.igor.igorDamageOverTimeEffects;
        Game.GetIgor().GetComponent<Igor>().EndPushingForLoad();
        Game.GetIgor().GetComponent<CommonEntity>()._lock = 0;

        Game.GetIgor().GetComponent<Inventory>().InventoryClear();
        ArrayList list = GetIInvItemArrayList(gd.igor.igorInventory);

        Game.GetIgor().GetComponent<Inventory>().FillInventoryFromSaveData(list);

    }

    /// <summary>
    /// recover InventoryItem from GameData
    /// if new item be inserted into this game ,please insert code for this item under "//ADD NEW ITEMS"
    /// </summary>
    /// <param name="list">some list</param>
    /// <returns>some array</returns>
    private static ArrayList GetIInvItemArrayList(List<GameData.IgorData.InventoryRecover> list)
    {
        ArrayList arrayList = new ArrayList();
        if (list == null)
        {
            arrayList = null;
            return arrayList;
        }
        else
        {
            foreach (GameData.IgorData.InventoryRecover irObject in list)
            {

                if (irObject.name == "Repair Kit")
                {
                    GameObject repairKitPrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("RepairKitPrefab"), new Vector3(0, -100, 0), Quaternion.identity);
                    RepairKit repairKit = repairKitPrefab.GetComponent<RepairKit>();
                    MonoBehaviour.Destroy(repairKitPrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(repairKit.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else if (irObject.name == "Energy capsule")
                {
                    GameObject energyCapsulePrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("EnergyCapsulePrefab"), new Vector3(0, -100, 0), Quaternion.identity);
                    EnergyCapsule energyCapsule = energyCapsulePrefab.GetComponent<EnergyCapsule>();
                    MonoBehaviour.Destroy(energyCapsulePrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(energyCapsule.PickUp(), irObject.amount);
                    arrayList.Add(ii);

                }
                else if (irObject.name == "Prism")
                {
                    GameObject prismPrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("PrismPrefab"), new Vector3(0, -100, 0), Quaternion.identity);
                    Prism prism = prismPrefab.GetComponent<Prism>();
                    MonoBehaviour.Destroy(prismPrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(prism.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else if (irObject.name == "Microwave")
                {
                    GameObject microwavePrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("MicrowavePrefab"), new Vector3(0, -100, 0), Quaternion.identity);
                    Microwave microwave = microwavePrefab.GetComponent<Microwave>();
                    MonoBehaviour.Destroy(microwavePrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(microwave.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else if (irObject.name == "Coil")
                {
                    GameObject coilPrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Coil"), new Vector3(0, -100, 0), Quaternion.identity);
                    Coil coil = coilPrefab.GetComponent<Coil>();
                    MonoBehaviour.Destroy(coilPrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(coil.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else if (irObject.name == "Fan")
                {
                    GameObject fanPrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Fan"), new Vector3(0, -100, 0), Quaternion.identity);
                    Fan fan = fanPrefab.GetComponent<Fan>();
                    MonoBehaviour.Destroy(fanPrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(fan.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else if (irObject.name == "Spring")
                {
                    GameObject springPrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Spring"), new Vector3(0, -100, 0), Quaternion.identity);
                    Spring spring = springPrefab.GetComponent<Spring>();
                    MonoBehaviour.Destroy(springPrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(spring.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else if (irObject.name == "USB Stick")
                {
                    GameObject usbStickPrefab = (GameObject)MonoBehaviour.Instantiate(Resources.Load("USBStickPrefab"), new Vector3(0, -100, 0), Quaternion.identity);
                    USBStick usbStick = usbStickPrefab.GetComponent<USBStick>();
                    MonoBehaviour.Destroy(usbStickPrefab);
                    Inventory.InventoryItem ii = new Inventory.InventoryItem(usbStick.PickUp(), irObject.amount);
                    arrayList.Add(ii);
                }
                else
                {
                }
                //ADD NEW ITEMS
                //eg: if(item.name == "the name of new item")
                //    {     
                //          arrayList.Add(item instance ,irOject.amount )
                //    }
            }
            return arrayList;
        }
    }

    #endregion
}
