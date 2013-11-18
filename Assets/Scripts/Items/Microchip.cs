using UnityEngine;
using System.Collections;

/// <summary>
/// microchip class
/// </summary>
public class Microchip : BaseItem
{
    /// <summary>
    /// particles that will be created when collecting the chip
    /// </summary>
    public GameObject expSparksPrefab;

    /// <summary>
    /// How much experience that the microchip has
    /// </summary>
    public int experienceValue = 1;

    /// <summary>
    /// called when picked up
    /// </summary>
    /// <returns>inventory item</returns>
    public override IInvItem PickUp()
    {
        if (experienceValue < 0)
        {
            Debug.LogError("experienceValue should > 0");
        }
        uint exp = (uint)experienceValue;
        Game.GetIgor().GetComponent<Igor>().AddExperience(exp);
        Instantiate(expSparksPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        return (IInvItem)null;
    }
}
