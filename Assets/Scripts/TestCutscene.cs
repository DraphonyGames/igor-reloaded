using UnityEngine;
using System.Collections;

/// <summary>
/// a random cutscene
/// </summary>
public class TestCutscene : MovementManager
{
    /// <summary>
    /// some object
    /// </summary>
    public GameObject toAnimate;

    /// <summary>
    /// the script
    /// </summary>
    public override void Script()
    {
        Append(new ActionSleep(5f));
        Append(new ActionTranslate(toAnimate, new Vector3(0f, 10f, 0f), 5f, true));

        IsRunning = true;
    }
}
