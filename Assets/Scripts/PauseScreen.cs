using UnityEngine;
using System.Collections;

/// <summary>
/// Class for a basic pause screen
/// </summary>
public class PauseScreen : MonoBehaviour
{
    /// <summary>
    /// True if the game is paused
    /// </summary>
    private bool isPaused;

    /// <summary>
    /// The texture to show as background
    /// </summary>
    private Texture2D background;

    /// <summary>
    /// Called when the Object is instantiated
    /// </summary>
    private void Start()
    {
        background = new Texture2D(1, 1);
        background.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
    }

    /// <summary>
    /// Called in every frame
    /// </summary>
    private void Update()
    {
        // Detect if we want to Pause or Unpause
        if (Input.GetButtonDown("Start"))
        {
            // Pause or Unpause
            if (isPaused)
            {
                Time.timeScale = 1f;
                Screen.showCursor = false;
            }
            else
            {
                Time.timeScale = 0f;
                Screen.showCursor = true;
            }

            isPaused = !isPaused;
        }
    }

    /// <summary>
    /// Called when the GUI should be drawn
    /// </summary>
    private void OnGUI()
    {
        if (isPaused)
        {
            // Darken the game
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
        }
    }
}
