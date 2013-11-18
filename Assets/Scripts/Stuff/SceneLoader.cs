using UnityEngine;
using System.Collections;

/// <summary>
/// Loads a scene on the point of the current game object
/// </summary>
public class SceneLoader : Triggerable
{

    /// <summary>
    /// name of the scene that should be loaded
    /// </summary>
    public string sceneName;

    /// <summary>
    /// possible trigger that should be called after loading
    /// </summary>
    public Triggerable otherTriggable;

    /// <summary>
    /// indicates whether the loader has been already triggered or not
    /// </summary>
    private bool triggered = false;

    /// <summary>
    /// seconds to wait until loading
    /// </summary>
    public float delay = 0f;

    /// <summary>
    /// Called by Unity
    /// </summary>
    public void Start()
    {
        //why don't destroy it on load?
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// called by the trigger when it's activated
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="isTriggered">whether triggered</param>
    public override void OnTrigger(GameObject by, bool isTriggered)
    {
        if (triggered)
        {
            return;
        }

        triggered = true;
        StartCoroutine("Load");
    }

    /// <summary>
    /// a loading coroutine
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator Load()
    {
        yield return new WaitForSeconds(delay);
        Application.LoadLevel(sceneName);

        if (otherTriggable)
        {
            otherTriggable.OnTrigger(gameObject, true);
        }
    }
}
