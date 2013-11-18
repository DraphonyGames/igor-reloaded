using UnityEngine;
using System.Collections;

/// <summary>
/// starting menu
/// </summary>
public class SplashScreen : MonoBehaviour
{
    /// <summary>
    /// Background texture
    /// </summary>
    public Texture2D backgroundTex = null;

    /// <summary>
    /// what to load
    /// </summary>
    public string levelToLoad = "Level0_1";

    /// <summary>
    ///  which skin
    /// </summary>
    public GUISkin mySkin;

    /// <summary>
    /// current what
    /// </summary>
    private int current = -1;

    /// <summary>
    /// some variable
    /// </summary>
    private float lastSwitch = 0;

    /// <summary>
    /// last position
    /// </summary>
    private Vector3 lastMousePos;

    /// <summary>
    /// Unity Awake
    /// </summary>
    private void Awake()
    {
        UnityEngine.Object[] objs = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject obj in objs)
        {
            if (obj.tag.Contains("Startscreen") || obj.tag.Equals("MainCamera") || obj.tag.Equals("BackgroundMusic"))
            {
                return;
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    /// <summary>
    /// Unity Start
    /// </summary>
    private void Start()
    {
        lastMousePos = Input.mousePosition;
        Screen.showCursor = true;
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

        GUI.SetNextControlName("0");
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 220, 250, 70), "Play"))
        {
            HandleButton(0);
        }

        GUI.SetNextControlName("1");
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 140, 250, 70), "Load"))
        {
            HandleButton(1);

        }

        GUI.SetNextControlName("2");
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 60, 250, 70), "Settings"))
        {
            HandleButton(2);
        }

        /*GUI.SetNextControlName("3");
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 20, 250, 70), "Credits"))
        {
            HandleButton(3);
        }*/

        GUI.SetNextControlName("4");
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 100, 250, 70), "Exit"))
        {
            HandleButton(4);
        }

        if (Time.time - lastSwitch >= 0.2)
        {
            if (Input.GetAxis("Vertical") <= -0.1 || Input.GetAxis("Mouse ScrollWheel") <= -0.1)
            {
                current++;
                if (current == 4)
                {
                    current = 0;
                }
                lastSwitch = Time.time;
            }
            else
            {
                if (Input.GetAxis("Vertical") >= 0.1 || Input.GetAxis("Mouse ScrollWheel") >= 0.1)
                {
                    current--;
                    if (current < -1)
                    {
                        current = 3;
                    }
                    lastSwitch = Time.time;
                }
            }
        }

        if (Input.GetButton("Jump") || Input.GetKeyDown(KeyCode.Return))
        {
            HandleButton(current);
        }

        if (lastMousePos != Input.mousePosition)
        {
            lastMousePos = Input.mousePosition;
            current = -1;
        }

        GUI.FocusControl(current + "");
    }

    /// <summary>
    /// some callback
    /// </summary>
    /// <param name="id">some ID</param>
    public void HandleButton(int id)
    {
        switch (id)
        {
            case 0:
                Application.LoadLevel("Level-1"); // Tutorial intro scene
                break;
            case 1:
                Application.LoadLevel("LoadGameScreen"); // the load game screen
                break;
            case 2:
                Application.LoadLevel("Settings"); // "Setting level"
                break;
            case 3:
                Application.LoadLevel("Credits");
                break;
            case 4:
                Application.Quit();
                break;
        }
    }
}
