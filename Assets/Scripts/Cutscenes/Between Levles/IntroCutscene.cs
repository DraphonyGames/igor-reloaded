using UnityEngine;
using System.Collections;

/// <summary>
/// the infamous intro cutscene class
/// </summary>
public class IntroCutscene : MovementManager
{
    /// <summary>
    /// Reference to the connection text
    /// </summary>
    public GameObject text1;

    /// <summary>
    /// Reference to the connected text
    /// </summary>
    public GameObject text2;

    /// <summary>
    /// Reference to the camera
    /// </summary>
    public GameObject usedCamera;

    /// <summary>
    /// Camera target
    /// </summary>
    public GameObject cameraTarget;

    /// <summary>
    /// Reference to Gil
    /// </summary>
    public GameObject gil;

    /// <summary>
    /// Reference to Igor
    /// </summary>
    public GameObject igor;

    /// <summary>
    /// Reference to Jeve
    /// </summary>
    public GameObject jeve;

    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        Append(new ActionSetCutscene(true));

        Append(new ActionTranslate(usedCamera, Vector3.right * 2, 5f, true));
        Append(new ActionSetLookAt(usedCamera, cameraTarget.transform.position));
        Sleep(0.2f);
        Append(new ActionSetLookAt(usedCamera, cameraTarget.transform.position));

        Append(new ActionAnimate(jeve, "AnimationIndex", 2));

        Sleep(2f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Sleep(1f);
        Append(new ActionRemove(text1));
        Append(new ActionRemove(text2));
        Append(new ActionScale(gil.transform, new Vector3(0.5f, 0.5f, 0.5f), 0.5f));
        Sleep(1f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Append(new ActionShowMessage("JEVE: Guten Morgen, Sir."));
        Sleep(1f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));

        Append(new ActionTranslate(usedCamera, new Vector3(357.1902f, 4.42088f, 353.2964f), 38f));
        Append(new ActionRotate(usedCamera, Vector3.down * 40, 38f));

        Sleep(2f);
        Append(new ActionAnimate(gil, "Animation", 1));
        Append(new ActionShowMessage("GIL: Hallo Jeve."));

        Append(new ActionShowMessage("GIL: Kommen wir gleich zur Sache, ich muss den Planeten für ein paar Wochen\n        verlassen und mein Raumschiff startet in einer halben Stunde."));

        Append(new ActionShowMessage("GIL: Ich habe die Konstruktionspläne für Projekt I.G.O.R. fertiggestellt.\n        Ich möchte, dass du sofort mit der Produktion beginnst!"));

        Sleep(14f);
        Append(new ActionAnimate(gil, "Animation", 0));
        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Append(new ActionShowMessage("JEVE: Sollten wir nicht vorher noch ein paar Tests machen? Vielleicht warten wir erst noch bis Sie wieder zurück sind."));
        Sleep(2f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Sleep(2f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Sleep(4f);
        Append(new ActionAnimate(gil, "Animation", 1));
        Append(new ActionShowMessage("GIL: Nein! I.G.O.R. wird mein Meisterwerk, ich kann nicht länger warten.\n        Das wird ein riesiger Schritt nach vorn für MacroRob."));
        Sleep(6f);
        Append(new ActionAnimate(gil, "Animation", 0));

        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Append(new ActionShowMessage("JEVE: Okay, ich starte sofort die Fertigung."));
        Sleep(2f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Sleep(3f);
        Append(new ActionAnimate(gil, "Animation", 1));
        Append(new ActionShowMessage("GIL: Das wollte ich hören."));
        
        Sleep(2f);

        Append(new ActionTranslate(usedCamera, Vector3.up * 1 + Vector3.forward * 10 + Vector3.right * 2, 7f, true));
//        Append(new ActionTranslate(usedCamera, new Vector3(347.1902f, 7.42088f, 368.2964f), 7f));

        Append(new ActionShowMessage("GILL: Okay, ich muss los. Ich verlass' mich auf Sie, Jeve."));

        Sleep(5f);
        Append(new ActionAnimate(gil, "Animation", 0));
        Append(new ActionAnimate(jeve, "AnimationIndex", 3));
        Append(new ActionShowMessage("JEVE: Ja, Sir."));
        Sleep(2f);
        Append(new ActionScale(gil.transform, new Vector3(0.005f, 0.005f, 0.005f), 0.5f));


        Append(new ActionAnimate(jeve, "AnimationIndex", 2));
        Append(new ActionShowMessage("[JEVE: Okay, dann mal an die Arbeit...]"));
        Sleep(3f);
        Append(new ActionScale(igor.transform, new Vector3(1.5f, 1.5f, 1.5f), 0.5f));
        Append(new ActionRotate(igor, new Vector3(0, 180, 0), 3f));
        Sleep(3f);

        Append(new ActionLoadScene("Tutorial"));

        IsRunning = true;
        Append(new ActionSetCutscene(false));
    }
}
