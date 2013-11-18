using UnityEngine;
using System.Collections;

/// <summary>
/// some loader class
/// </summary>
public class Loader : MonoBehaviour
{
    /// <summary>
    /// some load bar
    /// </summary>
    public Texture2D loadBar;

    /// <summary>
    /// current something
    /// </summary>
    private int current;
    /// <summary>
    /// no idea
    /// </summary>
    private int all;

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Start()
    {
        current = 0;
        all = 3;
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Update()
    {
        switch (current)
        {
            case 0:
                Application.LoadLevelAdditive("level1_0");
                break;
            case 1:
                Application.LoadLevelAdditive("level1_1");
                break;
            case 2:
                Application.LoadLevelAdditive("level1_2");
                break;
            default:
                Destroy(gameObject);
                break;
        }

        current++;
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void OnGUI()
    {
        ShowProgress(current, all);
    }

    /// <summary>
    /// shows stuff
    /// </summary>
    /// <param name="cur">at the moment</param>
    /// <param name="max">maximum number</param>
    private void ShowProgress(int cur, int max)
    {
        GUI.DrawTexture(new Rect(Screen.width - 150, Screen.height - 75, 100 / all * cur, 25), loadBar);
    }

}
