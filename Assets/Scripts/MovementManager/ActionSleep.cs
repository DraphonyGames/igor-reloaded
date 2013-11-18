using UnityEngine;
using System.Collections;

/// <summary>
/// Enables the MovementManager to sleep an amount of time
/// </summary>
public class ActionSleep : ActionGeneric
{
    /// <summary>
    /// Saves the remaining seconds to sleep
    /// </summary>
    private float sleep;

    /// <summary>
    /// Constructor for Sleep
    /// </summary>
    /// <param name="seconds">The number of seconds to sleep</param>
    public ActionSleep(float seconds)
    {
        sleep = seconds;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    public override void OnUpdate()
    {
        sleep -= Time.deltaTime;
    }

    /// <summary>
    /// Called to determine if the sequence is finished and should be dequeued
    /// </summary>
    /// <returns>True if it should be removed</returns>
    public override bool Finished()
    {
        return sleep < 0f;
    }
}
