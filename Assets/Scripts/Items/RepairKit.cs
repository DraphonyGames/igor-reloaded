using UnityEngine;
using System.Collections;

/// <summary>
/// Heals the player
/// </summary>
public class RepairKit : BaseItem
{
    /// <summary>
    /// Item for the Inventory
    /// </summary>
    public class InvRepairKit : IInvItem
    {
        /// <summary>
        /// Defines how much the player is healed
        /// </summary>
        public float amountToHeal;

        /// <summary>
        /// Prefab to instantiate when the player clicks the item
        /// </summary>
        public GameObject healPrefab;

        /// <summary>
        /// Texture to show in the inventory
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// Constructor for the inventory item
        /// </summary>
        /// <param name="amount">Number of hit points to heal</param>
        /// <param name="prefab">Prefab of the effect to play when healing</param>
        /// <param name="tex">Picture of the Item to show in the inventory</param>
        public InvRepairKit(float amount, GameObject prefab, Texture2D tex)
        {
            amountToHeal = amount;
            healPrefab = prefab;
            texture = tex;
        }

        /// <summary>
        /// Returns the name of the item
        /// </summary>
        /// <returns>Name of the item to show in the inventory</returns>
        public string GetDisplayName()
        {
            return "Repair Kit";
        }

        /// <summary>
        /// Maximal stacking size
        /// </summary>
        /// <returns>Maximal size of the item stack</returns>
        public int GetMaxStackSize()
        {
            return 99;
        }

        /// <summary>
        /// Called when the player tries to drop the item
        /// </summary>
        /// <returns>True if the drop was successful</returns>
        public bool Drop()
        {
            return false;
        }

        /// <summary>
        /// Called when the player tries to use the item
        /// </summary>
        /// <param name="entity">Instance of the player</param>
        /// <returns>True if it was used up</returns>
        public bool Use(GameObject entity)
        {
           
            entity.GetComponent<Igor>().InstantaneousHeal(amountToHeal);
            entity.GetComponent<Igor>().UseItemSelf(healPrefab);
            
            return true;
        }

        /// <summary>
        /// Returns if the item could be combined
        /// </summary>
        /// <param name="other">the other item</param>
        /// <returns>True, if it is combinable</returns>
        public bool Combinable(IInvItem other)
        {
            return false;
        }

        /// <summary>
        /// won't combine
        /// </summary>
        /// <param name="other">other object</param>
        /// <returns>some object</returns>
        public IInvItem Combine(IInvItem other)
        {
            return null;
        }

        /// <summary>
        /// returns texture
        /// </summary>
        /// <returns>the texture</returns>
        public Texture2D GetDrawable()
        {
            return texture;
        }
    }

    /// <summary>
    /// Defines how much the player is healed
    /// </summary>
    public float amountToHeal;

    /// <summary>
    /// Created when player heals himself
    /// </summary>
    public GameObject healPrefab;

    /// <summary>
    /// The texture to display in the inventory
    /// </summary>
    public Texture2D texture;

    /// <summary>
    /// makes repair kit pickable
    /// </summary>
    /// <returns>the inventory repair kit</returns>
    public override IInvItem PickUp()
    {
        if (pickUpEffect)
        {
            Instantiate(pickUpEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        return new InvRepairKit(amountToHeal, healPrefab, texture);
    }

    /// <summary>
    /// this object used for save/load "invRepairKit" into/from disk
    /// </summary>
    public InvRepairKit invRepairKit;

    /// <summary>
    /// constructor for repairKit
    /// this constructor used for save/load "invRepairKit" into/from disk
    /// </summary>
    public RepairKit()
    {
        invRepairKit = new InvRepairKit(amountToHeal, healPrefab, texture);
    }
}
