using UnityEngine;
using System.Collections;

/// <summary>
/// Action to reset the cutscene manager
/// </summary>
public class ActionReset : ActionGeneric
{
    /// <summary>
    /// The movement manager to reset
    /// </summary>
    private MovementManager movementManager;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="man">The movement manager to reset</param>
    public ActionReset(MovementManager man)
    {
        movementManager = man;
    }

    /// <summary>
    /// On start reset the movement manager
    /// </summary>
    public override void OnStart()
    {
        movementManager.StartManager();
        movementManager.enable = true;
    }

    /// <summary>
    /// Prevent it from running
    /// </summary>
    public override void OnFinish()
    {
        movementManager.IsRunning = false;
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
