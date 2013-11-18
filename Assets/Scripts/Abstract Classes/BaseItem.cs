using UnityEngine;
using System.Collections;

/// <summary>
/// Basic implementation for the IItem interface
/// </summary>
public abstract class BaseItem : MonoBehaviour
{
    /// <summary>
    /// Enables the rotation of the item
    /// </summary>
    public bool rotate = true;

    /// <summary>
    /// The rotation speed of the item (angle the object rotates within a second in degree)
    /// </summary>
    public float rotationSpeed = 90;

    /// <summary>
    /// The effect to create when the object is picked up
    /// </summary>
    public GameObject pickUpEffect;

    /// <summary>
    /// How much the item falls per second (not quadratic, since item usually don't fall a lot
    /// </summary>
    public float fallPerSecond = 25;

    /// <summary>
    /// Unity Start
    /// </summary>
    private void Start()
    {
        // turn into a trigger only after one second so that dropped items are not immediately collected
        // and the player has a chance to admire them
        collider.isTrigger = false;
        StartCoroutine(TurnIntoTrigger());
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // Rotate the item
        if (rotate)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        FallDown();
    }

    /// <summary>
    /// Lets the item fall (since the unity physics are somehow broken...)
    /// </summary>
    private void FallDown()
    {
        float dist = DistanceToGround();
        float maxFall = fallPerSecond * Time.deltaTime;

        if (dist < maxFall)
        {
            transform.Translate(0, dist * (-0.9f), 0, Space.World);
        }
        else if (dist != Mathf.Infinity)
        {
            transform.Translate(0, maxFall * (-1), 0, Space.World);
        }
    }

    /// <summary>
    /// Get the distance the entity is above something.
    /// </summary>
    /// <returns>The vertical distance to the next thing below the entity.</returns>
    private float DistanceToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(collider.bounds.center, -Vector3.up, out hit))
        {
            return collider.bounds.center.y - collider.bounds.extents.y - hit.point.y;
        }

        return Mathf.Infinity;
    }

    /// <summary>
    /// Enables the trigger of the collider after a certain time
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator TurnIntoTrigger()
    {
        yield return new WaitForSeconds(0.3f);
        collider.isTrigger = true;
    }

    /// <summary>
    /// Enables a game object to pick up another object
    /// </summary>
    /// <returns>The inventory id for the object</returns>
    public abstract IInvItem PickUp();
}
