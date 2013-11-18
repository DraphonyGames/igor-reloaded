using UnityEngine;
using System.Collections;

/// <summary>
/// Action for animating a game object
/// </summary>
public class ActionAnimate : ActionGeneric
{
    /// <summary>
    /// The game object to animate
    /// </summary>
    private GameObject gameObject;

    /// <summary>
    /// The name of the animation to change
    /// </summary>
    private string animation;

    /// <summary>
    /// The state the animation should be changed to
    /// </summary>
    private int state;

    /// <summary>
    /// Constructor of object
    /// </summary>
    /// <param name="gameObject">The game object to animate</param>
    /// <param name="animation">The name of the animation to change</param>
    /// <param name="state">The state the animation should be changed to</param>
    public ActionAnimate(GameObject gameObject, string animation, int state)
    {
        this.gameObject = gameObject;
        this.animation = animation;
        this.state = state;
    }

    /// <summary>
    /// Called when the action is dequeued
    /// </summary>
    public override void OnStart()
    {
        gameObject.GetComponent<Animator>().SetInteger(animation, state);
    }

    /// <summary>
    /// Called when finished
    /// </summary>
    /// <returns>Always return true.</returns>
    public override bool Finished()
    {
        return true;
    }
}
