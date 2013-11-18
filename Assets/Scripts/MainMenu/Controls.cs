using UnityEngine;
using System.Collections;

/// <summary>
/// some controls
/// </summary>
public class Controls : MonoBehaviour
{
    /// <summary>
    /// Background texture
    /// </summary>
    public Texture2D backgroundTex = null;

    /// <summary>
    /// some gui skin
    /// </summary>
    public GUISkin mySkin;

    /// <summary>
    /// current something
    /// </summary>
    private int current;

    /// <summary>
    /// a switch I believe
    /// </summary>
    private float lastSwitch;

    /// <summary>
    /// last mouse position
    /// </summary>
    private Vector3 lastMousePos;

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Start()
    {
        current = -1;
        lastMousePos = Input.mousePosition;
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void OnGUI()
    {
        // Draw background
        if (backgroundTex != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTex, ScaleMode.ScaleAndCrop);
        }

        GUI.skin = mySkin;
        GUI.Box(new Rect(Screen.width / 2 - 600, Screen.height / 2 - 220, 1200, 300), "");


        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 210, 150, 25), "Action");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 210, 150, 25), "Keyboard / Mouse");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 210, 150, 25), "Gamepad");


        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 170, 150, 25), "Movement");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 170, 150, 25), "W / A / S D");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 170, 150, 25), "Left Analog Stick");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 140, 150, 25), "Look Around");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 140, 150, 25), "Mouse");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 140, 150, 25), "Right Analog Stick");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 110, 150, 25), "Primary Attack");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 110, 150, 25), "Left Mouse Button");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 110, 150, 25), "3rd Trigger Right");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 80, 150, 25), "Secondary Attack");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 80, 150, 25), "Right Mouse Button");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 80, 150, 25), "3rd Trigger Left");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 50, 150, 25), "Jump");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 150, 25), "Space");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 50, 150, 25), "A");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 - 20, 150, 25), "Inventory");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 20, 150, 25), "I");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 - 20, 150, 25), "Y");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 + 10, 150, 25), "Use");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 10, 150, 25), "E");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 + 10, 150, 25), "B");

        GUI.Box(new Rect(Screen.width / 2 - 590, Screen.height / 2 + 40, 150, 25), "Skills");
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 40, 150, 25), "K");
        GUI.Box(new Rect(Screen.width / 2 + 300, Screen.height / 2 + 40, 150, 25), "LB");

        GUI.SetNextControlName("0");
        if (GUI.Button(new Rect(Screen.width / 2 - 175, Screen.height / 2 + 100, 350, 80), "Back to Settings"))
        {
            HandleButton(0);
        }

        if (Time.time - lastSwitch >= 0.2)
        {
            if (Input.GetAxis("Vertical") <= -0.1)
            {
                current++;
                if (current == 1)
                {
                    current = 0;
                    lastSwitch = Time.time;
                }
            }
            else
            {
                if (Input.GetAxis("Vertical") >= 0.1)
                {
                    current--;
                    if (current <= -1)
                    {
                        current = 0;
                        lastSwitch = Time.time;
                    }
                }
            }


            if (Input.GetButton("Jump"))
            {
                HandleButton(current);
            }

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
                Application.LoadLevel(3);
                break;
        }
    }
}
