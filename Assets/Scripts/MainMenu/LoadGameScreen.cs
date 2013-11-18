using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using System.Text;

/// <summary>
/// load game screen
/// </summary>
public class LoadGameScreen : MonoBehaviour
{
    /// <summary>
    /// Background texture
    /// </summary>
    public Texture2D backgroundTex = null;

    /// <summary>
    /// some GUI skin
    /// </summary>
    public GUISkin mySkin;

    /// <summary>
    /// some game data
    /// </summary>
    public GameData gameData;

    /// <summary>
    /// loading and saving
    /// </summary>
    public GameDataManager.SaveLoadDataShow[] saveLoadDatas = new GameDataManager.SaveLoadDataShow[10];

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Start()
    {
        Screen.showCursor = true;

        FileInfo saveDataFile = new FileInfo(Application.dataPath + "\\Save\\" + "savedata.dat");
        if (!saveDataFile.Exists)
        {
            for (int i = 0; i < saveLoadDatas.Length; i++)
            {
                saveLoadDatas[i] = new GameDataManager.SaveLoadDataShow("null");
            }
            XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "savedata.dat", XMLGameSaver.SerializeObject(saveLoadDatas));
        }
        else
        {
            saveLoadDatas = XMLGameSaver.DeserializeObject<GameDataManager.SaveLoadDataShow[]>(XMLGameSaver.LoadGame(Application.dataPath + "\\Save\\" + "savedata.dat"));
        }
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void OnGUI()
    {
        GUI.skin = mySkin;

        // Draw background
        if (backgroundTex != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTex, ScaleMode.ScaleAndCrop);
        }

        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        for (int i = 0; i < 10; i++)
        {
            if (i < 9)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 350, Screen.height / 16 + (i + 1) * Screen.height / 16, 700, Screen.height / 16), "   " + (i + 1) + ": " + saveLoadDatas[i].saveTime + " " + saveLoadDatas[i].sceneName))
                {
                    if (saveLoadDatas[i].sceneName == "null")
                    {

                    }
                    else
                    {

                        GameData gd = new GameData(GameData.Operation.LOAD);
                        gd = GameDataManager.Load((i + 1) + ".save");
                        GameDataRecover gdr = new GameDataRecover();
                        GameDataManager.SwapData sd = new GameDataManager.SwapData();
                        sd.initializeLoad = true;
                        sd.loadDataPath = (i + 1) + ".save";
                        XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "swap.dat", XMLGameSaver.SerializeObject(sd));
                        gdr.LoadScene(gd.scene);
                    }
                }
            }
            else
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 350, Screen.height / 16 + (i + 1) * Screen.height / 16, 700, Screen.height / 16), " " + (i + 1) + ": " + saveLoadDatas[i].saveTime + " " + saveLoadDatas[i].sceneName))
                {
                    if (saveLoadDatas[i].sceneName == "null")
                    {

                    }
                    else
                    {
                        GameData gd = new GameData(GameData.Operation.LOAD);
                        gd = GameDataManager.Load((i + 1) + ".save");
                        GameDataRecover gdr = new GameDataRecover();
                        GameDataManager.SwapData sd = new GameDataManager.SwapData();
                        sd.initializeLoad = true;
                        sd.loadDataPath = (i + 1) + ".save";
                        XMLGameSaver.SaveGame(Application.dataPath + "\\Save\\" + "swap.dat", XMLGameSaver.SerializeObject(sd));
                        gdr.LoadScene(gd.scene);
                    }
                }
            }
        }
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 16 + 10 * Screen.height / 16 + 40, 250, 60), "Main Menu"))
        {
            Application.LoadLevel("Startscreen");
        }

    }
}
