using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// cut scenes for level 0.4 which shows pictures
/// </summary>
public class JeveAndIgor : MovementManager
{
    /// <summary>
    /// pictures which will be shown
    /// </summary>
    public List<GameObject> pictures = new List<GameObject>();
   
    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        Append(new ActionSetCutscene(true, true));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[0].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[1].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[2].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[3].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[4].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[2].GetComponent<Renderer>(), true));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[2].GetComponent<Renderer>(), false));
        Append(new ActionSetRenderer(pictures[5].GetComponent<Renderer>(), false));
        Sleep(5f);

        Append(new ActionSetRenderer(pictures[6].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[7].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[8].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[9].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[10].GetComponent<Renderer>(), false));
        Sleep(5f);
        Append(new ActionSetRenderer(pictures[11].GetComponent<Renderer>(), false));
        Sleep(5f);

        Append(new ActionSetCutscene(false));
        Append(new ActionLoadScene("Level0.5"));

        IsRunning = true;
    }
}
