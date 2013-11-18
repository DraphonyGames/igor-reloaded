using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// a trigger will call "OnTrigger(GameObject by, bool isTriggered)" in any connected objects when the
/// trigger condition is met.
/// It is important to note that "by" can, in some cases, be "null"
/// </summary>
public interface ITrigger : IUsable
{
    /// <summary>
    /// used to set triggerable objects
    /// </summary>
    /// <param name="triggerable">object to add</param>
    void AddTriggerable(Triggerable triggerable);
    /// <summary>
    /// returns the current state of the trigger
    /// </summary>
    /// <returns>trigger state</returns>
    bool IsTriggered();
    /// <summary>
    /// if set to true, the trigger will only work once (for example: open a door and never close it again)
    /// </summary>
    /// <param name="makeOneShotTrigger">whether to make one shot</param>
    void SetOneShotTrigger(bool makeOneShotTrigger);
}
