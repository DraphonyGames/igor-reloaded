using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// triggering when all objects are destroyed
/// </summary>
public class DestructionTrigger : MonoBehaviour, ITrigger
{
    /// <summary>
    /// whether is triggered
    /// </summary>
    public bool isTriggered = false;

    /// <summary>
    /// if true, the trigger will only work once
    /// </summary>
    public bool isOneShotTrigger = true;

    /// <summary>
    /// one shot?
    /// </summary>
    private bool hasBeenTriggeredOnce = false;

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
    /// <param name="makeOneShot">if enable</param>
    public void SetOneShotTrigger(bool makeOneShot)
    {
        isOneShotTrigger = makeOneShot;
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
    /// list of objects that all need to be destroyed to trigger the action
    /// </summary>
    public List<GameObject> destructionObjects = new List<GameObject>();

    /// <summary>
    /// Unity Update
    /// </summary>
    private void Update()
    {
        if (isOneShotTrigger && hasBeenTriggeredOnce)
        {
            return;
        }

        int count = 0;
        foreach (GameObject obj in destructionObjects)
        {
            // note that in unity the == operator for GameObjects is overloaded:
            // when the object is destroyed, the actual reference does NOT become "null", however the "=="
            // operator still behaves as if it was "null"
            if (obj == null || (obj.tag.Contains("Enemy") && obj.GetComponent<CommonEntity>().GetFaction() == CommonEntity.Faction.PLAYER))
            {
                continue;
            }
            ++count;
        }

        if ((count == 0) != isTriggered)
        {
            OnUse(null);
        }
    }
}
