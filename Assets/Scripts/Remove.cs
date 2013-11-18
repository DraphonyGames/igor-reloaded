using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// removed all connected objects by Trigger
/// </summary>
public class Remove : Triggerable
{
    /// <summary>
    /// objects which will removed
    /// </summary>
    public List<GameObject> objekts = new List<GameObject>();

    /// <summary>
    /// called when triggered
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="isTriggered">whether triggered</param>
    public override void OnTrigger(GameObject by, bool isTriggered)
    {
        foreach (GameObject o in objekts)
        {
            Destroy(o.gameObject);
        }
    }
}
