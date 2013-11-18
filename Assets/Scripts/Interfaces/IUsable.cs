using UnityEngine;
using System.Collections;

/// <summary>
/// interface for all usable objects
/// </summary>
public interface IUsable
{
    /// <summary>
    /// called when the object is being used
    /// </summary>
    /// <param name="by">by who</param>
    void OnUse(GameObject by);
}
