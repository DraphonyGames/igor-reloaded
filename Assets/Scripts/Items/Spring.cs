using UnityEngine;
using System.Collections;

/// <summary>
/// Spring item
/// </summary>
public class Spring : BaseItem
{
    /// <summary>
    /// Item for the Inventory
    /// </summary>
    public class InvItem : IInvItem
    {
        /// <summary>
        /// name of the inventory item
        /// </summary>
        private string name;

        /// <summary>
        /// Create the inventory item with given name.
        /// </summary>
        /// <param name="_name">The name of the item.</param>
        public InvItem(string _name)
        {
            name = _name;
        }

        /// <summary>
        /// Returns the name of the item
        /// </summary>
        /// <returns>Name of the item to show in the inventory</returns>
        public string GetDisplayName()
        {
            return name;
        }

        /// <summary>
        /// Maximal stacking size
        /// </summary>
        /// <returns>Maximal size of the item stack</returns>
        public int GetMaxStackSize()
        {
            return 5;
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
            return false;
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
        /// Called, when the player tries to combine objects. Only called if combinable() returned true.
        /// </summary>
        /// <param name="other">Other object this object is about to be combined with</param>
        /// <returns>Name of the resulting inventory</returns>
        public IInvItem Combine(IInvItem other)
        {
            return null;
        }
    }

    /// <summary>
    /// pick up function
    /// </summary>
    /// <returns>the inventory </returns>
    public override IInvItem PickUp()
    {
        InvItem invItem = new InvItem(GetName());
        Destroy(gameObject);
        return invItem;
    }

    /// <summary>
    /// returns ne name of the item
    /// </summary>
    /// <returns>the name</returns>
    public virtual string GetName()
    {
        return "Spring";
    }
}
