using UnityEngine;
using System.Collections;

/// <summary>
/// renders stuff
/// </summary>
public class ActionSetRenderer : ActionGeneric
{
    /// <summary>
    /// The renderer to manipulate
    /// </summary>
    private Renderer renderer;

    /// <summary>
    /// The value to set the renderer to
    /// </summary>
    private bool value;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="renderer">The renderer to manipulate</param>
    /// <param name="value">The value to set the renderer to</param>
    public ActionSetRenderer(Renderer renderer, bool value)
    {
        this.renderer = renderer;
        this.value = value;
    }

    /// <summary>
    /// Set the renderer
    /// </summary>
    public override void OnStart()
    {
        renderer.enabled = value;
    }

    /// <summary>
    /// Immediately finished
    /// </summary>
    /// <returns>always true</returns>
    public override bool Finished()
    {
        return true;
    }
}
