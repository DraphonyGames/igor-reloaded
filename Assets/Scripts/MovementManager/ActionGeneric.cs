using UnityEngine;
using System.Collections;

/// <summary>
/// Used for ad-hoc creation of actions
/// </summary>
public abstract class ActionGeneric : IAction
{
    /// <summary>
    /// Virtual method to implement IAction
    /// </summary>
    public virtual void OnStart()
    {
    }

    /// <summary>
    /// Virtual method to implement IAction
    /// </summary>
    public virtual void OnFinish()
    {
    }

    /// <summary>
    /// Virtual method to implement IAction
    /// </summary>
    public virtual void OnUpdate()
    {
    }
    
    /// <summary>
    /// Virtual method to implement IAction
    /// </summary>
    public virtual void GUI()
    {
    }

    /// <summary>
    /// Necessary function
    /// </summary>
    /// <returns>if finished</returns>
    public abstract bool Finished();
}
