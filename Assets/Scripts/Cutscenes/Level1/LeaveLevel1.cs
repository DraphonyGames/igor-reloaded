using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cut scene when leaving first level
/// </summary>
public class LeaveLevel1 : MovementManager
{
    /// <summary>
    /// reference to igor
    /// </summary>
    public GameObject igor;

    /// <summary>
    /// reference to console
    /// </summary>
    public GameObject console;

    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        igor = Game.GetIgor();
        Append(new ActionLoadScene("Credits"));
    }
    
}
