using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// chest to get items from
/// </summary>
public class Chest : MonoBehaviour, IUsable
{
    /// <summary>
    /// script which will used on Items to animate the drop from the chest
    /// </summary>
    public UnityEngine.Object dropscript;

    /// <summary>
    /// items which will dropped by use the crate
    /// </summary>
    public List<GameObject> items = new List<GameObject>();

    /// <summary>
    /// flag for the open state of the crate
    /// </summary>
    public bool open = false;

    /// <summary>
    /// call by Unity
    /// </summary>
    public void Start()
    {
        if (open)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("open", open);
        }
    }

    /// <summary>
    /// create react on use
    /// </summary>
    /// <param name="by">by who</param>
    public void OnUse(GameObject by)
    {
        Igor igor = by.GetComponent<Igor>();
        if (igor != null)
        {
            if (!igor.PlayUseObjectInFrontAnimation(gameObject))
            {
                return;
            }
        }

        open = !open;
        Animator animator = GetComponent<Animator>();
        animator.SetBool("open", open);

        if (open)
        {
            StartCoroutine("ItemDrop");
        }
    }

    /// <summary>
    /// instantiate the items in the items list and add the animation to them
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator ItemDrop()
    {
        yield return new WaitForSeconds(0.2f);
        open = true;
        if (items.Count != 0)
        {
            foreach (GameObject o in items)
            {
                GameObject item = (GameObject)Instantiate(o, transform.position, Quaternion.identity);

                DropChest dropChest = (DropChest)item.AddComponent("DropChest");
                dropChest.direction = gameObject.transform.rotation * Vector3.back;
            }
            //delete the imtem list to prepare of droping again 
            items = new List<GameObject>();
        }
    }
}
