using UnityEngine;
using System.Collections;

/// <summary>
/// Prefabs implementing this Interface can be used in the inventory
/// The items' preview texture will be GetDisplayName()+"InventoryIcon" - it MUST exist in a Resources-folder
/// </summary>
public interface IInvItem
{
    /// <summary>
    /// The name of the object be be shown in the inventory view
    /// </summary>
    /// <returns>the display name</returns>
    string GetDisplayName();

    /// <summary>
    /// Maximal stacking size for this item.
    /// </summary>
    /// <returns>the size</returns>
    int GetMaxStackSize();

    /// <summary>
    /// Called when the player tries to drop this item.
    /// </summary>
    /// <returns>True, when the player can drop the item (will be removed from the inventory) of false, 
    /// when the item could not be dropped.</returns>
    bool Drop();

    /// <summary>
    /// Called when the player tries to use the item.
    /// </summary>
    /// <param name="entity">by who</param>
    /// <returns>True, when the player can use the item (will be removed from the inventory) of false,
    /// when the item could not be used.</returns>
    bool Use(GameObject entity);

    /// <summary>
    /// Determines if the two items are combinable.
    /// </summary>
    /// <param name="other">The other object.</param>
    /// <returns>True if the two items are combinable, false if not.</returns>
    bool Combinable(IInvItem other);

    /// <summary>
    /// Called, when the player tries to combine objects. Only called if combinable() returned true.
    /// </summary>
    /// <param name="other">Other object this object is about to be combined with</param>
    /// <returns>Name of the resulting inventory</returns>
    IInvItem Combine(IInvItem other);
}
