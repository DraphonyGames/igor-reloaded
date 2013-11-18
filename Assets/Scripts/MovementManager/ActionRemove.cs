using UnityEngine;
using System.Collections;

/// <summary>
/// Action used to remove an object
/// </summary>
public class ActionRemove : ActionGeneric
{
    /// <summary>
    /// Game object to remove
    /// </summary>
    private GameObject gameObject;

    /// <summary>
    /// Constructor of objects
    /// </summary>
    /// <param name="gameObject">game object to remove</param>
    public ActionRemove(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    /// <summary>
    /// Destroys the object on start
    /// </summary>
    public override void OnStart()
    {
        MonoBehaviour.Destroy(gameObject);
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
