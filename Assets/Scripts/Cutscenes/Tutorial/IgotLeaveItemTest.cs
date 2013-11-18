using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// leave item test area in the tutorial level
/// </summary>
public class IgotLeaveItemTest : MovementManager
{
    /// <summary>
    /// items which are must pick up before the scene start
    /// </summary>
    public List<GameObject> useItems = new List<GameObject>();

    /// <summary>
    /// trigger witch is enabled
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
                Append(new ActionShowMessage("Jeve: Du hast noch nicht alle RepairKits eingesammelt!"));

                Sleep(2f);
                Reset();
            }
            else
            {
                Append(new ActionSetCutscene(true));

                SwitchTrigger sw = (SwitchTrigger)TriggerEnable.GetComponent<SwitchTrigger>();
                sw.enable = true;

                Append(new ActionShowMessage("Jeve: Okay, dann zur nächsten Aufgabe."));
                Sleep(1f);

                Append(new ActionShowMessage("Jeve: Ich weiß, du bist eigentlich kein Kampfroboter, aber wir haben Probleme mit ein paar durchgedrehten Robotern."));
                Sleep(1f);

                Append(new ActionShowMessage("Jeve: Zerstöre bitte die Gegner auf der anderen Seite der Tür, wir treffen uns dann im nächsten Raum."));
                Sleep(4f);

                Append(new ActionShowMessage("Jeve: Wenn du beschädigt bist, drücke I um im Inventar ein paar RepairKits zu benutzen."));
                Sleep(4f);

                Append(new ActionShowMessage("Jeve: Du kannst Inventarobjekte auch in die Schnellzugriffsleiste ziehen, um sie direkt benutzen zu können."));
                Sleep(6f);

                Append(new ActionSetCutscene(false));
                run = true;
            }
        }
    }
}
