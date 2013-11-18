using UnityEngine;
using System.Collections;

/// <summary>
/// Settings class
/// </summary>
public class MySettings : MonoBehaviour
{
    /// <summary>
    /// Background texture
    /// </summary>
    public Texture2D backgroundTex = null;

    /// <summary>
    /// new skin
    /// </summary>
    public GUISkin mySkin;

    /// <summary>
    /// text for sound button
    /// </summary>
    private string sound = "Sound on";

    /// <summary>
    /// The current selected menu item
    /// </summary>
    private int current = -1;

    /// <summary>
    /// Time (float) of the last switch
    /// </summary>
    private float lastSwitch = 0;

    /// <summary>
    /// The last mouse position.
    /// </summary>
    private Vector3 lastMousePos;

    /// <summary>
    /// is the button pressed
    /// </summary>
    private bool isPressed = false;

    /// <summary>
    /// The timing.
    /// </summary>
    private float timing = 0;

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
        lastMousePos = Input.mousePosition;
        audio.mute = true;
    }

    /// <summary>
    /// Raises the GU event.
    /// </summary>
    private void OnGUI()
    {
        GUI.skin = mySkin;
        Color color = GUI.color;

        // Draw background
        if (backgroundTex != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTex, ScaleMode.ScaleAndCrop);
        }

        GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 135, 200, 80), "Difficulty:");

        GUI.color = Game.currentDifficulty == Game.Difficulty.Medium ? Color.red : color;
        GUI.SetNextControlName("0");
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 160, 80), "medium"))
        {
            HandleButton(0);
        }


        GUI.color = Game.currentDifficulty == Game.Difficulty.Hard ? Color.red : color;
        GUI.SetNextControlName("1");
        if (GUI.Button(new Rect(Screen.width / 2 + 130, Screen.height / 2 - 150, 160, 80), "hard"))
        {
            HandleButton(1);
        }

        GUI.color = color;
        GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 60, 200, 80), "Volume:");
        GUI.SetNextControlName("2");
        Game.CurrentVolume = GUI.HorizontalSlider(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 10, 250, 10), Game.CurrentVolume, 0f, 1f);

        GUI.SetNextControlName("3");
        if (GUI.Button(new Rect(Screen.width / 2 + 30, Screen.height / 2 - 50, 250, 80), "Controls"))
        {
            HandleButton(3);
        }

        GUI.SetNextControlName("4");
        if (GUI.Button(new Rect(Screen.width / 2 - 230, Screen.height / 2 + 40, 250, 80), "Main Menu"))
        {
            //Application.LoadLevel("Startscreen");
            HandleButton(4);
        }

        // not every frame input
        if (Time.time - lastSwitch >= 0.2)
        {
            if (Input.GetAxis("Vertical") <= -0.1 || Input.GetAxis("Mouse ScrollWheel") <= -0.1 || Input.GetAxis("Horizontal") >= 0.1 ||
                Input.GetAxis("Mouse ScrollWheel Left/Right") >= 0.1)
            {
                current++;
                if (current == 5)
                {
                    current = 0;
                }
                lastSwitch = Time.time;
            }
            else
            {
                if (Input.GetAxis("Vertical") >= 0.1 || Input.GetAxis("Mouse ScrollWheel") >= 0.1 || Input.GetAxis("Horizontal") <= -0.1 ||
                    Input.GetAxis("Mouse ScrollWheel Left/Right") <= -0.1)
                {
                    current--;
                    if (current < -1)
                    {
                        current = 4;
                    }
                    lastSwitch = Time.time;
                }
            }
            if (Input.GetButton("Jump") && !isPressed)
            {
                HandleButton(current);
                isPressed = true;
            }
        }

        if (Time.time - timing >= 0.7)
        {
            isPressed = false;
            timing = Time.time;
        }

        if (lastMousePos != Input.mousePosition)
        {
            lastMousePos = Input.mousePosition;
            current = -1;
        }

        GUI.FocusControl(current.ToString());
    }

    /// <summary>
    /// Handles the button.
    /// </summary>
    /// <param name='id'>
    /// id of the button
    /// </param>
    public void HandleButton(int id)
    {
        switch (id)
        {
            case 0:
                Game.currentDifficulty = Game.Difficulty.Medium;
                break;
            case 1:
                Game.currentDifficulty = Game.Difficulty.Hard;
                break;
            case 2:
                if (sound == "Sound on")
                {
                    audio.mute = false;
                    sound = "Sound off";
                }
                else
                {
                    audio.mute = true;
                    sound = "Sound on";
                }
                break;
            case 3:
                Application.LoadLevel("Controls"); // Show the Controlls Overview
                break;

            case 4:
                Application.LoadLevel("Startscreen"); // 1 = StartScreen
                break;
        }
    }
}
