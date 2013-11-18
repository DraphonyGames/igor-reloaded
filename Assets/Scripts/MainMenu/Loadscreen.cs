using UnityEngine;
using System.Collections;

/// <summary>
/// loading screen class
/// </summary>
public class Loadscreen : MonoBehaviour
{
    /// <summary>
    /// Background texture
    /// </summary>
    public Texture2D backgroundTex = null;

    /// <summary>
    /// some timer
    /// </summary>
    public float timer = 3;

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Start()
    {
        StartCoroutine("DisplayScene");
    }

    /// <summary>
    /// Raises the GU event.
    /// </summary>
    private void OnGUI()
    {
        // Draw background
        if (backgroundTex != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTex, ScaleMode.ScaleAndCrop);
        }
    }

    /// <summary>
    /// some timer
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator DisplayScene()
    {
        yield return new WaitForSeconds(timer);
        Application.LoadLevel("Startscreen");
    }
}
