using UnityEngine;
using System.Collections;

/// <summary>
/// moves towards something
/// </summary>
public class ActionMoveTo : IAction
{
    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    private bool relative;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    private float duration;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    private float remaining;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    private Vector3 start;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    private Vector3 to;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    private GameObject gameObject;

    /// <summary>
    /// some constructor
    /// </summary>
    /// <param name="gameObject">game object</param>
    /// <param name="to">to where</param>
    /// <param name="duration">over time</param>
    /// <param name="relative">whether relative</param>
    public ActionMoveTo(GameObject gameObject, Vector3 to, float duration, bool relative)
    {
        this.to = to;
        this.start = gameObject.transform.position;
        this.duration = duration;
        this.relative = relative;
        this.gameObject = gameObject;
    }

    /// <summary>
    /// some constructor
    /// </summary>
    /// <param name="gameObject">game object</param>
    /// <param name="to">to where</param>
    /// <param name="duration">over time</param>
    public ActionMoveTo(GameObject gameObject, Vector3 to, float duration)
    {
        this.to = to;
        this.duration = duration;
        this.relative = false;
        this.gameObject = gameObject;
    }

    /// <summary>
    /// some constructor
    /// </summary>
    /// <param name="gameObject">game object</param>
    /// <param name="to">to where</param>
    public ActionMoveTo(GameObject gameObject, Vector3 to)
    {
        this.to = to;
        this.duration = 0f;
        this.relative = false;
        this.gameObject = gameObject;
    }

    /// <summary>
    /// some callback
    /// </summary>
    public void OnStart()
    {
        if (relative)
        {
            to = gameObject.transform.position + to;
        }

        remaining = duration;
    }

    /// <summary>
    /// some callback
    /// </summary>
    public void OnFinish()
    {
    }

    /// <summary>
    /// some callback
    /// </summary>
    public void OnUpdate()
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
            //gameObject.transform.Translate((to - start) * (delta / duration), Space.World);
            gameObject.transform.position = (to - start) * (delta / duration);
        }
    }

    /// <summary>
    /// some callback
    /// </summary>
    public void GUI()
    {
    }

    /// <summary>
    /// some callback
    /// </summary>
    /// <returns>if finished</returns>
    public bool Finished()
    {
        return remaining <= 0f;
    }
}
