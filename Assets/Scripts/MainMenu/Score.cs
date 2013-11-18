using UnityEngine;
using System.Collections;

/// <summary>
/// some score
/// </summary>
public class Score : MonoBehaviour
{
    /// <summary>
    /// some skin
    /// </summary>
    public GUISkin mySkin;

    /// <summary>
    /// last mouse position
    /// </summary>
    private Vector3 lastMousePos;

    /// <summary>
    /// current something
    /// </summary>
    private int current = -1;

    /// <summary>
    /// some switch
    /// </summary>
    private float lastSwitch = 0;

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Start()
    {
        lastMousePos = Input.mousePosition;
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void OnGUI()
    {
        GUI.skin = mySkin;

        GUI.SetNextControlName("0");
        if (GUI.Button(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 40, 250, 80), "Main Menu"))
        {

            HandleButton(0);
        }

        if (Time.time - lastSwitch >= 0.2)
        {
            if (Input.GetAxis("Vertical") <= -0.1 || Input.GetAxis("Mouse ScrollWheel") <= -0.1 || Input.GetAxis("Horizontal") >= 0.1 ||
                Input.GetAxis("Mouse ScrollWheel Left/Right") >= 0.1)
            {
                current++;
                if (current == 1)
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
                    if (current < 0)
                    {
                        current = 0;
                    }
                    lastSwitch = Time.time;
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
    /// <param name="id">some button ID</param>
    public void HandleButton(int id)
    {
        switch (id)
        {
            case 0:
                Application.LoadLevel(1); // 1 = Startscreen
                break;
        }
    }
}
