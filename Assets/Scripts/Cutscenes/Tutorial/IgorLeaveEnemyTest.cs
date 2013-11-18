using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script to allow/disallow to leave Igor the enemy test in the tutorial level
/// </summary>
public class IgorLeaveEnemyTest : MovementManager
{
    /// <summary>
    /// items which are must pick up before the scene start
    /// </summary>
    public List<GameObject> useItems = new List<GameObject>();

    /// <summary>
    /// trigger which is enabled
    /// </summary>
    public GameObject TriggerEnable;

    /// <summary>
    /// reference to igor
    /// </summary>
    public GameObject igor;

    /// <summary>
    /// reference to Jeve
    /// </summary>
    public GameObject jeve;

    /// <summary>
    /// flag which will be set by the first play to prepare scene that it is running only one time
    /// </summary>
    private bool run = false;

    /// <summary>
    /// flag for skill tutorial only once 
    /// </summary>
    private bool skilTutorialDone = false;

    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        if (!run)
        {
            bool allPickedUp = true;
            foreach (GameObject item in useItems)
            {
                if (item != null)
                {
                    allPickedUp = false;
                }
            }

            if (!allPickedUp)
            {
                Append(new ActionShowMessage("Igor: [Hmmm? Ich habe noch nicht alle Gegner getötet.]"));

                Sleep(2f);
                Reset();
            }
            else
            {
                if (Game.GetIgor().GetComponent<Igor>().skillpoints == 0)
                {
                    SwitchTrigger sw = (SwitchTrigger)TriggerEnable.GetComponent<SwitchTrigger>();
                    sw.enable = true;
                    run = true;
                }
                else
                {
                    if (!skilTutorialDone)
                    {
                        Append(new ActionSetCutscene(true));
                        Append(new ActionShowMessage("IGOR: [Vielleicht lässt sich mit diesen Microchips was anfangen. Ich schau mal in meine Bauanleitung]"));
                        Sleep(4f);
                        Append(new ActionShowMessage("IGOS 4.2 Readme: \"ACHTUNG! IGOR nicht mit Wasser füllen... \n ...nutze Funktion K (K-Taste für externe Anwender) um in den Modifizierungsmodus zu gelangen... \""));
                        Sleep(4f);
                        Append(new ActionShowMessage("IGOS Readme: \" ...wähle eine beliebige Verbesserung und werte sie mit eingesammelten Speichermedien auf \n Achtung!: manche Funktionen bedürfen weiterer Komponenten... \""));
                        Sleep(1f);
                        Append(new ActionSetCutscene(false));
                        skilTutorialDone = true;
                        Sleep(2f);
                        SwitchTrigger sw = (SwitchTrigger)TriggerEnable.GetComponent<SwitchTrigger>();
                        sw.enable = true;
                        run = true;
                    }
                    else
                    {
                        Append(new ActionShowMessage("IGOR: [Vielleicht habe ich noch nicht alles aufgewertet]"));
                        Sleep(1f);

                    }
                    Reset();
                }
            }
        }
    }
}
