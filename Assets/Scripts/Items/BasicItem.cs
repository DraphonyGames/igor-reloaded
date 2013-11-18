using UnityEngine;
using System.Collections;

/// <summary>
/// basic item class with some standard methods
/// </summary>
public class BasicItem : Prism
{
    /// <summary>
    /// the name will be used to load icon etc..
    /// </summary>
    public string itemName;
    
    /// <summary>
    /// returns a name
    /// </summary>
    /// <returns>the name</returns>
    public override string GetName()
    {
        return itemName;
    }
}
