using UnityEngine;
using System.Collections;

/// <summary>
/// Heals the players mana
/// </summary>
public class EnergyCapsule : BaseItem
{
    /// <summary>
    /// Item for the Inventory
    /// </summary>
    public class InvEnergyCapsule : IInvItem
    {
        /// <summary>
        /// Defines how much the player is healed
        /// </summary>
        private float amountToHeal;

        /// <summary>
        /// Prefab to instantiate when the player clicks the item
        /// </summary>
        private GameObject healPrefab;

        /// <summary>
        /// Constructor for the inventory item
        /// </summary>
        /// <param name="amount">Number of mana points to heal</param>
        /// <param name="prefab">Prefab of the effect to play when healing</param>
        public InvEnergyCapsule(float amount, GameObject prefab)
        {
            amountToHeal = amount;
            healPrefab = prefab;
        }

        /// <summary>
        /// Returns the name of the item
        /// </summary>
        /// <returns>Name of the item to show in the inventory</returns>
        public string GetDisplayName()
        {
            return "Energy capsule";
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

            entity.GetComponent<Igor>().InstantaneousManaRegeneration(amountToHeal);
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
        /// try to combine
        /// </summary>
        /// <param name="other">other object</param>
        /// <returns>some item</returns>
        public IInvItem Combine(IInvItem other)
        {
            return null;
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
    /// makes Energy Capsules pickable
    /// </summary>
    /// <returns>the inventory repair kit</returns>
    public override IInvItem PickUp()
    {
        if (pickUpEffect)
        {
            Instantiate(pickUpEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        return new InvEnergyCapsule(amountToHeal, healPrefab);
    }
}
