using UnityEngine;
using System.Collections;

/// <summary>
/// Action to rotate a game object
/// </summary>
public class ActionRotate : ActionGeneric
{
    /// <summary>
    /// The game object to rotate
    /// </summary>
    private GameObject gameObject;

    /// <summary>
    /// The rotation to perform
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// The start of the rotation
    /// </summary>
    private Vector3 start;

    /// <summary>
    /// The direction of the rotation
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// The duration of the rotation (in sec)
    /// </summary>
    private float duration;

    /// <summary>
    /// The remaining seconds of the rotation
    /// </summary>
    private float remaining;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="gameObject">The game object to rotate</param>
    /// <param name="rotation">The rotation to perform</param>
    /// <param name="duration">The duration of the rotation (in sec)</param>
    public ActionRotate(GameObject gameObject, Vector3 rotation, float duration)
    {
        this.gameObject = gameObject;
        this.rotation = rotation;
        this.duration = duration;
    }

    /// <summary>
    /// Called on start
    /// </summary>
    public override void OnStart()
    {
        start = gameObject.transform.rotation.eulerAngles;
        remaining = duration;

        direction = (start - rotation) * -1;

        if (direction.x > 180f)
        {
            direction.x -= 360;
        }

        if (direction.y > 180f)
        {
            direction.y -= 360;
        }

        if (direction.y > 180f)
        {
            direction.y -= 360;
        }

        if (direction.x < -180f)
        {
            direction.x += 360;
        }

        if (direction.y < -180f)
        {
            direction.y += 360;
        }

        if (direction.y < -180f)
        {
            direction.y += 360;
        }

    }

    /// <summary>
    /// Called every frame
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

        gameObject.transform.Rotate(direction * (delta / duration));
    }

    /// <summary>
    /// Calculates if we are finished
    /// </summary>
    /// <returns>True when remaining le 0</returns>
    public override bool Finished()
    {
        return remaining <= 0f;
    }
}
