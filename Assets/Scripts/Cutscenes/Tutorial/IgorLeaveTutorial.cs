using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// last cut scene in the tutorial
/// </summary>
public class IgorLeaveTutorial : MovementManager
{
    /// <summary>
    /// items which are must pick up before the scene start
    /// </summary>
    public List<GameObject> useItems = new List<GameObject>();

    /// <summary>
    /// reference to igor
    /// </summary>
    public GameObject igor;

    /// <summary>
    /// reference to Jeve
    /// </summary>
    public GameObject jeve;

    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        Append(new ActionSetCutscene(true));
        Append(new ActionShowMessage("Jeve: Okay, es scheint alles zu funktionieren!"));
        Sleep(2f);
        
        Append(new ActionShowMessage("Jeve: Genug für heute!"));
        Sleep(5f);
        Append(new ActionShowMessage("Jeve: Achso, die Repaikits muss ich dir leider nochmal abnehmen, die gehören zu dieser Anlage."));
        
        Sleep(5f);
        Append(new ActionShowMessage("Jeve: Komm, ich zeig' dir die Anlage..."));
        
        Sleep(3f);
        Append(new ActionSetCutscene(false));
        Append(new ActionLoadScene("Level0.4"));
    }
}
