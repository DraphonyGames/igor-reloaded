using UnityEngine;
using System.Collections;

/// <summary>
/// Igor leaves the igor maker - easy as that :D
/// </summary>
public class IgorLeaveIgorMaker : MovementManager
{
    /// <summary>
    /// reference to Jeve
    /// </summary>
    public GameObject jeve;

    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        GameObject igor = Game.GetIgor();

        Append(new ActionSetCutscene(true));

        // Igor out of igor maker

        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Sleep(4f);

        Append(new ActionRotate(igor, new Vector3(0, 90, 0), 1f));
        Sleep(1f);

        Append(new ActionAnimate(igor, "AnimationIndex", 1));
        Append(new ActionTranslate(igor, Vector3.forward * 5, 1f, true));
        Sleep(1f);

        Append(new ActionTranslate(igor, Vector3.forward * 9.2f, 1f, true));
        Sleep(1f);
        Append(new ActionRotate(igor, new Vector3(0, 135, 0), 1f));
        Sleep(1f);
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(1f);
        
        // Dialog

        Append(new ActionRotate(jeve, new Vector3(0, -45, 0), 1f));
        Append(new ActionShowMessage("Jeve: Hallo IGOR! Ich bin Jeve, dein Erbauer."));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Sleep(4f);

        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Append(new ActionShowMessage("IGOR: Brrrsrizl!"));
        Sleep(3f);

        Append(new ActionShowMessage("Jeve: Oh, dein Sprachprozessor scheint noch nicht gestartet zu sein. Einen Moment bitte..."));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Sleep(6f);

        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Sleep(4f);

        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Append(new ActionShowMessage("IGOR: ... Hallo. Jeve."));
        Sleep(2f);

        Append(new ActionShowMessage("Jeve: Sehr schön."));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Sleep(3f);

        Append(new ActionShowMessage("Jeve: Kommunikation funktionstüchtig."));
        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Sleep(4f);

        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Append(new ActionShowMessage("IGOR: I.G.O.S. Version 4.2. Systemcheck OK."));
        Sleep(6f);

        Append(new ActionShowMessage("Jeve: Betriebssystem funktionstüchtig."));
        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Sleep(4f);

        Append(new ActionShowMessage("Jeve: Perfekt, dann kann es ja los gehen!"));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Sleep(4f);

        Append(new ActionRotate(jeve, new Vector3(0, 0, 0), 1f));
        Append(new ActionShowMessage("Jeve: Du solltest dich mit deinem neuen Chassis vertraut machen. Folge mir."));
        Sleep(4f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 1));

        // Jeve through door

        Append(new ActionTranslate(jeve, Vector3.forward * 12, 1f, true));
        Sleep(1f);
        Append(new ActionRotate(igor, new Vector3(0, 0, 0), 0.5f));
        Sleep(0.2f);

        Append(new ActionRotate(jeve, new Vector3(0, -30, 0), 0.5f));
        Append(new ActionTranslate(jeve, Vector3.forward * 20, 2f, true));
        Sleep(2f);
        Append(new ActionRotate(jeve, new Vector3(0, 270, 0), 0.5f));
        Sleep(0.5f);

        Append(new ActionTranslate(jeve, Vector3.forward * 5, 0.5f, true));
        Sleep(0.5f);
        Append(new ActionRotate(jeve, new Vector3(0, 90, 0), 0.5f));
        Sleep(0.5f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));

        Append(new ActionSetCutscene(false));
        Append(new ActionSetKinematic(igor, false));

        Append(new ActionShowMessage("Jeve: Versuche mal auf die andere Seite des Raumes zu kommen!"));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Sleep(4f);

        Append(new ActionShowMessage("Jeve: Halte die Leertaste gedrückt, um deinen Jet zu aktivieren."));
        Sleep(4f);

        Append(new ActionAnimate(jeve, "AnimationIndex", 0));

        IsRunning = true;
    }
}
