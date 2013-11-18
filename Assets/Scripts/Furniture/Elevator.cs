using UnityEngine;
using System.Collections;

/// <summary>
/// elevator room script
/// </summary>
public class Elevator : Triggerable
{
    /// <summary>
    /// for internal use
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// Enables the elevator
    /// </summary>
    public bool enableElevator = true;

    /// <summary>
    /// state of the door
    /// </summary>
    public bool isOpen = true;

    /// <summary>
    /// for preventing race conditions within an animation
    /// </summary>
    private bool inAnimation = false;

    /// <summary>
    /// Collider for the front doors
    /// </summary>
    private Collider frontCollider = null;

    /// <summary>
    /// run on start
    /// </summary>
    public void Start()
    {
        animator = GetComponent<Animator>();
        BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            if (collider.name.Equals("Wall__Front_"))
            {
                frontCollider = collider;
            }
        }

        if (isOpen)
        {
            StartCoroutine("RunOpeningAnimation");
        }
        else
        {
            StartCoroutine("RunClosingAnimation");
        }
    }

    /// <summary>
    /// called by the trigger when it's activated
    /// </summary>
    /// <param name="by">the game object that calls the trigger</param>
    /// <param name="isTriggered">indicates whether it's triggered or not</param>
    public override void OnTrigger(GameObject by, bool isTriggered)
    {
        if (!inAnimation && enableElevator)
        {
            if (isOpen)
            {
                StartCoroutine("RunClosingAnimation");
            }
            else
            {
                StartCoroutine("RunOpeningAnimation");
            }
        }
    }

    /// <summary>
    /// runs closing animation
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator RunClosingAnimation()
    {
        inAnimation = true;
        frontCollider.enabled = true;
        animator.SetBool("close", true);
        isOpen = false;
        yield return new WaitForSeconds(0f);
        inAnimation = false;
    }

    /// <summary>
    /// runs opening animation
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator RunOpeningAnimation()
    {
        inAnimation = true;
        animator.SetBool("close", false);
        yield return new WaitForSeconds(1f);
        frontCollider.enabled = false;
        isOpen = true;
        inAnimation = false;
    }
}
