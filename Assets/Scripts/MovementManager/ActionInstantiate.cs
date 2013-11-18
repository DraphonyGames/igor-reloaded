using UnityEngine;
using System.Collections;

/// <summary>
/// Instantiates a game object in the movement manager
/// </summary>
public class ActionInstantiate : ActionGeneric
{
    /// <summary>
    /// Object to instantiate
    /// </summary>
    private GameObject gameObject;

    /// <summary>
    /// The position of the newly instantiated object
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// The rotation of the newly instantiated object
    /// </summary>
    private Quaternion rotation;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="gameObject">game object to instantiate</param>
    /// <param name="position">the position</param>
    /// <param name="rotation">the rotation</param>
    public ActionInstantiate(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        this.gameObject = gameObject;
        this.position = position;
        this.rotation = rotation;
    }

    /// <summary>
    /// Called on start, instantiates the given game object
    /// </summary>
    public override void OnStart()
    {
        GameObject.Instantiate(gameObject, position, rotation);
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
