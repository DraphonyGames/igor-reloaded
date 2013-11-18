using UnityEngine;
using System.Collections;

/// <summary>
/// Action to move objects
/// </summary>
public class ActionTranslate : ActionGeneric
{
    /// <summary>
    /// Relative movement
    /// </summary>
    private bool relative;

    /// <summary>
    /// The duration in seconds
    /// </summary>
    private float duration;

    /// <summary>
    /// The remaining time in seconds
    /// </summary>
    private float remaining;

    /// <summary>
    /// Start of the movement
    /// </summary>
    private Vector3 start;

    /// <summary>
    /// Goal of the movement
    /// </summary>
    private Vector3 to;

    /// <summary>
    /// Game object to translate
    /// </summary>
    private GameObject gameObject;

    /// <summary>
    /// Construction of object
    /// </summary>
    /// <param name="gameObject">Game object to translate</param>
    /// <param name="to">Goal of the movement</param>
    /// <param name="duration">The duration in seconds</param>
    /// <param name="relative">Relative movement</param>
    public ActionTranslate(GameObject gameObject, Vector3 to, float duration, bool relative)
    {
        this.to = to;
        this.duration = duration;
        this.relative = relative;
        this.gameObject = gameObject;
    }

    /// <summary>
    /// More easy translation
    /// </summary>
    /// <param name="gameObject">>Game object to translate</param>
    /// <param name="to">Goal of the movement</param>
    /// <param name="duration">The duration in seconds</param>
    public ActionTranslate(GameObject gameObject, Vector3 to, float duration)
    {
        this.to = to;
        this.duration = duration;
        this.relative = false;
        this.gameObject = gameObject;
    }

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="gameObject">>Game object to translate</param>
    /// <param name="to">Goal of the movement</param>
    public ActionTranslate(GameObject gameObject, Vector3 to)
    {
        this.to = to;
        this.duration = 0f;
        this.relative = false;
        this.gameObject = gameObject;
    }

    /// <summary>
    /// Called on start
    /// </summary>
    public override void OnStart()
    {
        start = gameObject.transform.position;

        if (relative)
        {
            to = gameObject.transform.position + to;
        }

        remaining = duration;
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

        if (gameObject != null)
        {
            gameObject.transform.Translate((to - start) * (delta / duration), relative ? Space.Self : Space.World);
        }
    }

    /// <summary>
    /// Finished when done
    /// </summary>
    /// <returns>True if remaining is not greater than 0</returns>
    public override bool Finished()
    {
        return remaining <= 0f;
    }
}
