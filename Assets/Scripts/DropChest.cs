using UnityEngine;
using System.Collections;

/// <summary>
/// Animate a item by leaving a chest
/// </summary>
public class DropChest : MonoBehaviour
{
    /// <summary>
    /// direction in which the item will move after leaving the box
    /// </summary>
    public Vector3 direction = new Vector3(0, 0, 1);

    /// <summary>
    /// current direction of the move
    /// </summary>
    private Vector3 currentDirection = new Vector3(0, 4.5f, 0);

    /// <summary>
    /// start scale of the animation
    /// </summary>
    private float scale = 0.1f;

    /// <summary>
    /// end scale of the animation
    /// </summary>
    private float maxScale = 1;

    /// <summary>
    /// unity default function
    /// </summary>
    private void Start()
    {
        maxScale = transform.localScale.y;
        transform.localScale = new Vector3(scale, scale, scale);
        StartCoroutine("ItemDrop");
    }

    /// <summary>
    /// unity default function
    /// </summary>
    private void Update()
    {

        //scale- and moveanimation of the imtem
        if (scale < maxScale)
        {
            scale = scale + (1 / (maxScale * 3)) * Time.deltaTime;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.Translate(currentDirection * (1 / (maxScale * 2)) * 10 * Time.deltaTime, Space.World);
        }
        else
        {
            if (scale > maxScale)
            {
                scale = maxScale;
            }
        }


    }

    /// <summary>
    /// delay function for the forward movement (waiting to reach the chest  border before moving out)
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator ItemDrop()
    {
        yield return new WaitForSeconds(0.3f);
        currentDirection = direction * 2;
    }
}
