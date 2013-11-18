using UnityEngine;
using System.Collections;

/// <summary>
/// a door class
/// </summary>
public class Door : Triggerable
{
    /// <summary>
    /// plays once when the door moves
    /// </summary>
    public AudioClip doorOpenSound;

    /// <summary>
    /// plays once when the door moves
    /// </summary>
    public AudioClip doorCloseSound;
    
    /// <summary>
    /// for internal use
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// state of the door
    /// </summary>
    private bool isOpen = false;

    /// <summary>
    /// called by the trigger when it's activated
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="isTriggered">if triggered</param>
    public override void OnTrigger(GameObject by, bool isTriggered)
    {
        if (!isOpen && isTriggered)
        {
            audio.PlayOneShot(doorOpenSound);
        }
        else if (isOpen && !isTriggered)
        {
            audio.PlayOneShot(doorCloseSound);
        }

        isOpen = isTriggered;
        animator.SetBool("isOpen", isOpen);
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
