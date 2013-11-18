using UnityEngine;
using System.Collections;

/// <summary>
/// this is an abstract class rather than an interface, so that in Unity a List of Triggerable
/// will be displayed in the Inspector correctly so that it is possible to directly drag&amp;drop objects there
/// </summary>
public abstract class Triggerable : MonoBehaviour
{
    /// <summary>
    /// called by the trigger when it's activated
    /// </summary>
    /// <param name="by">by object</param>
    /// <param name="isTriggered">whether triggered</param>
    public abstract void OnTrigger(GameObject by, bool isTriggered);
}
