using UnityEngine;
using System.Collections;

/// <summary>
/// Action to show a message on the screen
/// </summary>
public class ActionShowMessage : ActionGeneric
{
    /// <summary>
    /// The message to show
    /// </summary>
    private string message;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="message">Message to show</param>
    public ActionShowMessage(string message)
    {
        this.message = message;
    }

    /// <summary>
    /// On start show message
    /// </summary>
    public override void OnStart()
    {
        MessageBoard.AddMessage(message, null, true);
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
