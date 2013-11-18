using UnityEngine;
using System.Collections;

/// <summary>
/// destroys the own game object when triggered
/// </summary>
public class SelfDestructor : Triggerable
{
    /// <summary>
    /// should be called when the object "by" wants to use the trigger (via a usage action)
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="isTriggered">whether triggered</param>
    public override void OnTrigger(GameObject by, bool isTriggered)
    {
        Destroy(gameObject);
    }
}
