using UnityEngine;
using System.Collections;

/// <summary>
/// Action for loading a new scene from a cutscene
/// </summary>
public class ActionLoadScene : ActionGeneric
{
    /// <summary>
    /// Name of the level to load
    /// </summary>
    private string levelName;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="levelName">Name of the level to load</param>
    public ActionLoadScene(string levelName)
    {
        this.levelName = levelName;
    }

    /// <summary>
    /// Called on start
    /// </summary>
    public override void OnStart()
    {
        Application.LoadLevel(levelName);
    }

    /// <summary>
    /// Immediately finished
    /// </summary>
    /// <returns>always true</returns>
    public override bool Finished()
    {
        return true;
    }
}
