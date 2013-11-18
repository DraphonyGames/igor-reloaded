using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using System.Text;

/// <summary>
/// manages game data
/// </summary>
public class GameDataManager : MonoBehaviour
{   
    /// <summary>
    /// the GameData instance should be saved
    /// </summary>
    public GameData gameData;

    /// <summary>
    /// ten save / load slot
    /// </summary>
    public SaveLoadDataShow[] saveLoadDatas = new SaveLoadDataShow[10];

    /// <summary>
    /// whether exist the swap data
    /// </summary>
    public bool hasSwapDate;

    /// <summary>
    /// the GameData instance should be loaded
    /// </summary>
    public GameData loadGameDate = new GameData(GameData.Operation.LOAD);

    /// <summary>
    /// GameDataRecover instance for recover game
    /// </summary>
    public GameDataRecover gdr;

    /// <summary>
    /// the swap data used for change scene
    /// </summary>
    public SwapData sd;

    /// <summary>
    /// swaps data
    /// </summary>
    public class SwapData
    {   
        /// <summary>
        /// whether load a game stand when this class be initialized
        /// </summary>
        public bool initializeLoad;

        /// <summary>
        /// the path of file that should be loaded
        /// </summary>
        public string loadDataPath;

        /// <summary>
        /// null constructor of SwapData
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public SwapData()
        {
        }

        /// <summary>
        /// constructor of SwapData
        /// </summary>
        /// <param name="iLoad">whether load when initialized</param>
        /// <param name="path">path of file.</param>
        public SwapData(bool iLoad, string path)
        {
            loadDataPath = path;
            initializeLoad = iLoad;
        }
    }
    /// <summary>
    /// call by Unity
    /// </summary>
    public void Awake()
    {
        Time.timeScale = 1;
        Game.IsPaused = false;
        Game.isMenuOpen = false;
  //    GameDataRecover gdr = new GameDataRecover();

        if (!Directory.Exists(Application.dataPath + "\\Save"))
        {
            Directory.CreateDirectory(Application.dataPath + "\\Save");
        }

        FileInfo swapfile = new FileInfo(Application.dataPath + "\\Save\\" + "swap.dat");
        FileInfo saveDataFile = new FileInfo(Application.dataPath + "\\Save\\" + "savedata.dat");

        if (!saveDataFile.Exists)
        {
            for (int i = 0; i < saveLoadDatas.Length; i++)
            {
                saveLoadDatas[i] = new SaveLoadDataShow("null");
            }
            XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "savedata.dat", XMLGameSaver.SerializeObject(saveLoadDatas));
        }
        else
        {
            saveLoadDatas = XMLGameSaver.DeserializeObject<SaveLoadDataShow[]>(XMLGameSaver.LoadGame(Application.dataPath + "\\Save\\" + "savedata.dat"));
        }

        if (swapfile.Exists)
        {
            sd = XMLGameSaver.DeserializeObject<SwapData>(XMLGameSaver.LoadGame(Application.dataPath + "\\Save\\" + "swap.dat"));
            if (sd.initializeLoad == true)
            {
                hasSwapDate = true;

                loadGameDate = Load(sd.loadDataPath);
                
                sd.initializeLoad = false;
                XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "swap.dat", XMLGameSaver.SerializeObject(sd));
                swapfile.Delete();
            }
        }
    }

    /// <summary>
    /// call by Unity
    /// </summary>
    private void Start()
    {

        if (hasSwapDate)
        {
            GameDataRecover gdr = new GameDataRecover();
  //          Skills.InstantiateSkills(); // initial skill initiation if there are no skill objects
  //          Time.timeScale = 0;
            gdr.CutscenePrefabRecover(loadGameDate);
            gdr.GameDifficultyDataRecover(loadGameDate);
            gdr.IgorRecover(loadGameDate);
            gdr.SkillBarRecover(loadGameDate);
            gdr.SkillTreeRecover(loadGameDate);
            gdr.ScenePrefabRecover(loadGameDate);
            gdr.BossDataRecover(loadGameDate);
  //          Time.timeScale = 1;
            hasSwapDate = false;
        }


    }

    /// <summary>
    /// save a game state
    /// </summary>
    /// <param name="fileName">the name of save file</param>
    public void Save(string fileName)
    {
        string gameDataFile = Application.dataPath + "\\Save\\" + fileName;
        //      string gameDataFile = Application.dataPath + "\\" + dataFileName;
        string dataString = XMLGameSaver.SerializeObject(gameData);
        XMLGameSaver.SaveGame(gameDataFile, dataString);
    }

    /// <summary>
    /// load a game state
    /// </summary>
    /// <param name="fileName">the name of file</param>
    /// <returns>a GameData object </returns>
    public static GameData Load(string fileName)
    {

        // string gameDataFile = Application.dataPath + "\\" + fileName;
        string gameDataFile = Application.dataPath + "\\Save\\" + fileName;
        string dataString = XMLGameSaver.LoadGame(gameDataFile);
        GameData gamedataFormXML = XMLGameSaver.DeserializeObject<GameData>(dataString);

        return gamedataFormXML;
    }

    /// <summary>
    /// call by Unity
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonUp("Start"))
        {
            if (Game.IsCutscene)
            {
                return;
            }

            if (isPaused == true && isMainMenu == true)
            {
                Screen.showCursor = false;
                isPaused = false;
                Time.timeScale = 1;
                Game.IsPaused = false;
                Game.isMenuOpen = false;
            }
            else if (Game.isMenuOpen && isMainMenu == true)
            {
                return;
            }
            else if (isPaused == true && (isSaveMenu == true || isLoadMenu == true))
            {

                isSaveMenu = false;
                isLoadMenu = false;
                isMainMenu = true;

            }
            else
            {
                isPaused = true;
                Game.IsPaused = true;
                Time.timeScale = 0;
                Game.isMenuOpen = true;
            }
        }


    }

    #region gui 
    /// <summary>
    /// whether the game is paused
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// whether in MainMenu
    /// </summary>
    private bool isMainMenu = true;

    /// <summary>
    /// whether in SaveMenu
    /// </summary>
    private bool isSaveMenu = false;

    /// <summary>
    /// whether in LoadMenu
    /// </summary>
    private bool isLoadMenu = false;

    /// <summary>
    /// GUI of save menu
    /// </summary>
    private void OnGUI()
    {
        if (isPaused == true && isMainMenu == true)
        {
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            Screen.showCursor = true;
            int vwidth = (int)(Screen.width * 0.3);
            if (vwidth < 175)
            {
                vwidth = 175;
            }
            int vheight = (int)(Screen.width * 0.3);
            if (vheight < 175)
            {
                vheight = 175;
            }

            int vleft = (Screen.width - vwidth) / 2;
            int vhigh = (Screen.height - vheight) / 2;

            GUI.Box(new Rect(vleft, vhigh, vwidth, vheight), "Option");

            if (GUI.Button(new Rect(vleft + 10, vhigh + 3 * vheight / 17, vwidth - 20, vheight / 5), "Save"))
            {
                /*
                isPaused = false;
                Time.timeScale = 1;
                gameData = new GameData(GameData.operation.SAVE);
                Save();
                 */
                isMainMenu = false;
                isSaveMenu = true;
            }
            if (GUI.Button(new Rect(vleft + 10, vhigh + 3 * vheight / 17 + vheight / 5, vwidth - 20, vheight / 5), "Load"))
            {
                isMainMenu = false;
                isLoadMenu = true;
            }
            if (GUI.Button(new Rect(vleft + 10, vhigh + 3 * vheight / 17 + 2 * vheight / 5, vwidth - 20, vheight / 5), "Main Menu"))
            {
                isPaused = false;
                Game.IsPaused = false;
                Time.timeScale = 1;
                Skills.SkillsClear();
                Application.LoadLevel(1);
            }
            if (GUI.Button(new Rect(vleft + 10, vhigh + 3 * vheight / 17 + 3 * vheight / 5, vwidth - 20, vheight / 5), "Back To Game"))
            {
                Screen.showCursor = false;
                isPaused = false;
                Game.IsPaused = false;
                Game.isMenuOpen = false;
                Time.timeScale = 1;
            }
        }

        if (isPaused == true && isSaveMenu == true)
        {
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            int vwidth = (int)(Screen.width * 0.4);
            if (vwidth < 175)
            {
                vwidth = 175;
            }
            int vheight = (int)(Screen.width * 0.3);
            if (vheight < 175)
            {
                vheight = 175;
            }

            int vleft = (Screen.width - vwidth) / 2;

            GUI.Box(new Rect(vleft, 10, vwidth, Screen.height - 20), "SAVE GAME");

            bool saved = false;
            for (int i = 0; i < 10; i++)
            {
                if (GUI.Button(new Rect(vleft + 5, 10 + (i + 1) * (Screen.height - 20) / 11, vwidth - 10, (Screen.height - 20) / 11), " " + (i + 1) + ": " + saveLoadDatas[i].saveTime + " " + saveLoadDatas[i].sceneName))
                {
                    gameData = new GameData(GameData.Operation.SAVE);
                    Save((i + 1) + ".save");
                    saveLoadDatas[i] = new SaveLoadDataShow(gameData);
                    saved = true;
                }
            }

            if (saved)
            {
                XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "savedata.dat", XMLGameSaver.SerializeObject(saveLoadDatas));
            }
        }
        if (isPaused == true && isLoadMenu == true)
        {
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            int vwidth = (int)(Screen.width * 0.4);
            if (vwidth < 175)
            {
                vwidth = 175;
            }
            int vheight = (int)(Screen.width * 0.3);
            if (vheight < 175)
            {
                vheight = 175;
            }

            int vleft = (Screen.width - vwidth) / 2;

            GUI.Box(new Rect(vleft, 10, vwidth, Screen.height - 20), "LOAD GAME");
            for (int i = 0; i < 10; i++)
            {
                if (GUI.Button(new Rect(vleft + 5, 10 + (i + 1) * (Screen.height - 20) / 11, vwidth - 10, (Screen.height - 20) / 11), " " + (i + 1) + ": " + saveLoadDatas[i].saveTime + " " + saveLoadDatas[i].sceneName))
                {
                    if (saveLoadDatas[i].sceneName == "null")
                    {

                    }
                    else
                    {
                        isPaused = false;
                        isLoadMenu = false;
                        GameData gd = new GameData(GameData.Operation.LOAD);
                        gd = Load((i + 1) + ".save");
                        GameDataRecover gdr = new GameDataRecover();
                        SwapData sd = new SwapData();
                        sd.initializeLoad = true;
                        sd.loadDataPath = (i + 1) + ".save";
                        XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "swap.dat", XMLGameSaver.SerializeObject(sd));
                        gdr.LoadScene(gd.scene);
                    }
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// the save/load slot show
    /// </summary>
    public SaveLoadDataShow[] showData;

    /// <summary>
    /// show save data in the GUI
    /// </summary>
    public class SaveLoadDataShow
    {   
        /// <summary>
        /// the slot number
        /// </summary>
        public string indexNum;

        /// <summary>
        /// the scene name of saved data
        /// </summary>
        public string sceneName;

        /// <summary>
        /// the igor level of saved data
        /// </summary>
        public string igorLevel;

        /// <summary>
        /// the saving time of a save data
        /// </summary>
        public string saveTime;

        /// <summary>
        /// null constructor of SaveLoadDataShow
        /// Attention: the null constructor is very important for serialization!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public SaveLoadDataShow()
        {
        }

        /// <summary>
        /// constructor of SaveLoadDataShow
        /// </summary>
        /// <param name="str">the show string</param>
        public SaveLoadDataShow(string str)
        {
            sceneName = str;
            saveTime = " ";
        }
        /// <summary>
        /// constructor of SaveLoadDataShow
        /// </summary>
        /// <param name="gd">the GameData</param>
        public SaveLoadDataShow(GameData gd)
        {
            sceneName = gd.scene;
            // igorLevel = gd.igor
            saveTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }
    }
}
