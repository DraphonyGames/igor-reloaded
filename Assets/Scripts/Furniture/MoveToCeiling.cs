using UnityEngine;
using System.Collections;

/// <summary>
/// Moves the object to the ceiling. Pretty basic routine... No collider detection, etc, only doing a raycast
/// at THE MIDDLE of the object upwards
/// </summary>
public class MoveToCeiling : MonoBehaviour
{
    /// <summary>
    /// How fast the object moves to the ceiling
    /// </summary>
    public float moveSpeed = 2;

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Start()
    {
        gameObject.transform.Translate(Vector3.down * 1); // just to be sure: move lamp down
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Update()
    {
        Vector3 basevec = transform.position + (Vector3.up * 0.4f * gameObject.collider.bounds.size.y);

        RaycastHit hit;
        float distanceToCeiling = 1000;

        if (Physics.Raycast(basevec, Vector3.up, out hit))
        {
            distanceToCeiling = hit.distance;
        }
        else
        {
            Destroy(this);
        }

        /* just some security measure */
        if (distanceToCeiling > 0.3f)
        {
            transform.Translate(Vector3.up * (distanceToCeiling - 0.15f));
        }

        Destroy(this); // remove this script, so we don't do the detecion all the time
    }
}
