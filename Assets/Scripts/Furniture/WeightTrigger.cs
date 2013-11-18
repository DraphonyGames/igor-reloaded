using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// WeightTrigger: Put a PushableBox or some rigidbody on top to trigger it
/// </summary>
public class WeightTrigger : SwitchTrigger
{
    /// <summary>
    /// mass that has to be reached to trigger the events
    /// </summary>
    public float triggeringMass = 1f;

    /// <summary>
    /// objects currently on the trigger
    /// </summary>
    private List<GameObject> objectsCurrentlyInside = new List<GameObject>();

    /// <summary>
    /// check mass and/or exec trigger
    /// </summary>
    private void CheckMass()
    {
        float currentMass = 0f;

        foreach (GameObject obj in objectsCurrentlyInside)
        {
            if (obj == null)
            {
                continue;
            }
            Rigidbody rigid = obj.GetComponent<Rigidbody>();
            if (!rigid)
            {
                continue;
            }
            currentMass += rigid.mass;
        }

        bool shouldBeTriggered = currentMass >= triggeringMass;
        if (shouldBeTriggered != IsTriggered())
        {
            OnUse(null);
        }
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    /// <param name="collision">set by Unity</param>
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        // ignore non-rigid bodies..
        if (other.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        objectsCurrentlyInside.Add(other);
        CheckMass();
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    /// <param name="collision">set by Unity</param>
    private void OnCollisionExit(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        // ignore non-rigid bodies..
        if (other.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        objectsCurrentlyInside.Remove(other);
        CheckMass();
    }
}
