using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Switch, allows to trigger stuff
/// </summary>
public class SwitchTrigger : MonoBehaviour, ITrigger
{
    /// <summary>
    /// Flag for enabling the trigger
    /// </summary>
    public bool enable = true;

    /// <summary>
    /// for internal use
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// is it triggered?
    /// </summary>
    public bool isTriggered = false;

    /// <summary>
    /// whether the trigger will only work once
    /// </summary>
    public bool isOneShotTrigger = false;

    /// <summary>
    /// only needed when isOneShotTrigger
    /// </summary>
    private bool hasBeenTriggeredOnce = false;

    /// <summary>
    /// used to set triggerable objects via Unity
    /// </summary>
    public List<Triggerable> connectedObjects;

    /// <summary>
    /// used to set triggerable objects via Unity
    /// </summary>
    /// <param name="triggerable">Object to add to trigger list</param>
    public void AddTriggerable(Triggerable triggerable)
    {
        connectedObjects.Add(triggerable);
    }

    /// <summary>
    /// sets whether the trigger will only work once
    /// </summary>
    /// <param name="makeOneShot">set it to one shot or not</param>
    public void SetOneShotTrigger(bool makeOneShot)
    {
        isOneShotTrigger = makeOneShot;
    }

    /// <summary>
    /// should be called when the object "by" wants to use the trigger (via a usage action: IUsable)
    /// </summary>
    /// <param name="by">the object which triggered this switch</param>
    public void OnUse(GameObject by)
    {
        if (!enable)
        {
            return;
        }

        if (isOneShotTrigger && hasBeenTriggeredOnce)
        {
            return;
        }

        // we know it's an Igor anyway
        Igor igor;
        if ((by != null) && (igor = by.GetComponent<Igor>()))
        {
            if (!igor.PlayUseObjectInFrontAnimation(gameObject))
            {
                return;
            }
        }

        isTriggered = !isTriggered;
        hasBeenTriggeredOnce = true;

        foreach (Triggerable obj in connectedObjects)
        {
            if (obj == null)
            {
                continue;
            }

            obj.OnTrigger(by, isTriggered);
        }

        audio.Play();
        if (animator)
        {
            animator.SetBool("isTriggered", isTriggered);
        }

    }

    /// <summary>
    /// returns the current state of the trigger
    /// </summary>
    /// <returns>state of the trigger</returns>
    public bool IsTriggered()
    {
        return isTriggered;
    }

    /// <summary>
    /// Unity Start
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
