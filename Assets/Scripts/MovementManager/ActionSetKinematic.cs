using UnityEngine;
using System.Collections;

/// <summary>
/// Enables or disables kinematic movement for an object
/// </summary>
public class ActionSetKinematic : ActionGeneric
{
    /// <summary>
    /// Object to set
    /// </summary>
    private GameObject obj;

    /// <summary>
    /// Value to set isKinematic to
    /// </summary>
    private bool value;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="gameObject">An object with a Rigidbody</param>
    /// <param name="to">Value to set isKinematic to</param>
    public ActionSetKinematic(GameObject gameObject, bool to)
    {
        obj = gameObject;
        value = to;
    }

    /// <summary>
    /// Called, when the action is dequeued
    /// </summary>
    public override void OnStart()
    {
        Rigidbody comp = obj.GetComponent<Rigidbody>();
        if (comp == null)
        {
            Debug.LogWarning("ActionSetKinematic: The given gameObject " + obj.name + " has no RigidBody attached to it.");
        }
        else
        {
            comp.isKinematic = value;
        }
    }

    /// <summary>
    /// Should return true when the action should be removed
    /// </summary>
    /// <returns>always true</returns>
    public override bool Finished()
    {
        return true;
    }
}
