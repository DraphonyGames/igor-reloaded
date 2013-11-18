using UnityEngine;
using System.Collections;

/// <summary>
/// Action to let object look at each other
/// </summary>
public class ActionSetLookAt : ActionGeneric
{
    /// <summary>
    /// The game object to rotate
    /// </summary>
    private GameObject me;

    /// <summary>
    /// The coordinates to look at
    /// </summary>
    private Vector3 coord;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="me">The game object to rotate</param>
    /// <param name="coord">The coordinates to look at</param>
    public ActionSetLookAt(GameObject me, Vector3 coord)
    {
        this.me = me;
        this.coord = coord;
    }

    /// <summary>
    /// Called on start
    /// </summary>
    public override void OnStart()
    {
        me.transform.LookAt(coord);
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
