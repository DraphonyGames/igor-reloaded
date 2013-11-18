using UnityEngine;
using System.Collections;

/// <summary>
/// Action to scale a game object
/// </summary>
public class ActionScale : ActionGeneric
{
    /// <summary>
    /// The game object to scale
    /// </summary>
    private Transform gameObject;

    /// <summary>
    /// The scale
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// The relative scale
    /// </summary>
    private Vector3 relative;

    /// <summary>
    /// The duration in sec
    /// </summary>
    private float duration;

    /// <summary>
    /// The remaining seconds
    /// </summary>
    private float remaining;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="gameObject">The game object to scale</param>
    /// <param name="scale">The scale vector</param>
    /// <param name="duration">The duration</param>
    public ActionScale(Transform gameObject, Vector3 scale, float duration)
    {
        this.gameObject = gameObject;
        this.scale = scale;
        this.duration = duration;
    }

    /// <summary>
    /// Called on start
    /// calculates the relative scale
    /// </summary>
    public override void OnStart()
    {
        remaining = duration;
        relative = scale - gameObject.localScale;
    }

    /// <summary>
    /// Calculates the new scale
    /// </summary>
    public override void OnUpdate()
    {
        float delta;

        if (remaining < Time.deltaTime)
        {
            delta = remaining;
        }
        else
        {
            delta = Time.deltaTime;
        }

        remaining -= delta;

        gameObject.localScale += relative * (delta / duration);
    }

    /// <summary>
    /// Returns if the action is finished
    /// </summary>
    /// <returns>True if remaining le 0</returns>
    public override bool Finished()
    {
        return remaining <= 0f;
    }
}
