using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// a walk-on trigger
/// </summary>
public class FloorTrigger : MonoBehaviour, ITrigger
{
    /// <summary>
    /// Tag which need a game object to activate the Trigger
    /// </summary>
    public new string tag = "Unit";

    /// <summary>
    /// current state
    /// </summary>
    public bool isTriggered = false;

    /// <summary>
    /// if true, the trigger will only work once
    /// </summary>
    public bool isOneShotTrigger = false;

    /// <summary>
    /// one shot?
    /// </summary>
    private bool hasBeenTriggeredOnce = false;

    /// <summary>
    /// the radius will be set automatically to fit the size of the plane
    /// </summary>
    private float radius = 0f;

    /// <summary>
    /// after a quick check against the sphere, check against the following bounds
    /// </summary>
    private Bounds actualCheckingBounds;

    /// <summary>
    /// used to set triggerable objects via Unity
    /// </summary>
    public List<Triggerable> connectedObjects;

    /// <summary>
    /// used to set triggerable objects
    /// </summary>
    /// <param name="triggerable">to add</param>
    public void AddTriggerable(Triggerable triggerable)
    {
        connectedObjects.Add(triggerable);
    }

    /// <summary>
    /// should be called when the object "by" wants to use the trigger (via a usage action)
    /// </summary>
    /// <param name="by">by who</param>
    public void OnUse(GameObject by)
    {
        isTriggered = !isTriggered;
        hasBeenTriggeredOnce = true;

        foreach (Triggerable obj in connectedObjects)
        {
            obj.OnTrigger(by, isTriggered);
        }
    }

    /// <summary>
    /// returns the current state of the trigger
    /// </summary>
    /// <returns>if triggered</returns>
    public bool IsTriggered()
    {
        return isTriggered;
    }

    /// <summary>
    /// sets whether the trigger will only work once
    /// </summary>
    /// <param name="makeOneShot">whether to make one shot</param>
    public void SetOneShotTrigger(bool makeOneShot)
    {
        isOneShotTrigger = makeOneShot;
    }

    /// <summary>
    /// Unity Start
    /// </summary>
    private void Start()
    {
        // remove the mesh renderer component when the game starts - this allows for easier placement in the editor
        Destroy(GetComponent<MeshRenderer>());

        // generate radius for quick-check and actual bounding box
        radius = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 10.0f;
        actualCheckingBounds = new Bounds(transform.position, 10f * transform.localScale);
        actualCheckingBounds.Encapsulate(new Vector3(transform.position.x, transform.position.y + 100f, transform.position.z));
        actualCheckingBounds.Encapsulate(new Vector3(transform.position.x, transform.position.y - 100f, transform.position.z));

        if (tag.Equals(""))
        {
            tag = "Unit";
        }
    }

    /// <summary>
    /// Unity Update
    /// </summary>
    private void Update()
    {
        if (radius == 0)
        {
            return;
        }
        if (isOneShotTrigger && hasBeenTriggeredOnce)
        {
            return;
        }

        // look for "Unit"s in a range
        GameObject found = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < hitColliders.Length; ++i)
        {
            Collider other = hitColliders[i];

            // check actual projected bounding box of the plane and whether the object should trigger
            if (actualCheckingBounds.Contains(other.gameObject.transform.position) && other.tag.Contains(tag))
            {

                found = other.gameObject;
                break;
            }
        }

        if ((bool)found != isTriggered)
        {
            OnUse(found);
        }
    }
}
