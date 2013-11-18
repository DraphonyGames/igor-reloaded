using UnityEngine;
using System.Collections;

/// <summary>
/// Action to set isCutscene in Game
/// </summary>
public class ActionSetCutscene : ActionGeneric
{
    /// <summary>
    /// Value to set Game.isCutscene to
    /// </summary>
    private bool value;

    /// <summary>
    /// Ignore the black bars
    /// </summary>
    private bool ignore;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="to">Value to set Game.isCutscene to</param>
    public ActionSetCutscene(bool to)
    {
        value = to;
        ignore = false;
    }

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="to">Value to set Game.isCutscene to</param>
    /// <param name="ignore">Disable the black bars</param>
    public ActionSetCutscene(bool to, bool ignore)
    {
        value = to;
        this.ignore = ignore;
    }

    /// <summary>
    /// Called on start
    /// </summary>
    public override void OnStart()
    {
        Game.IsCutscene = value;
        Game.ignoreBars = ignore;
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
