using UnityEngine;
using System.Collections;

/// <summary>
/// some random cutscene
/// </summary>
public class IgorLeaveJump : MovementManager
{
    /// <summary>
    /// reference to igor
    /// </summary>
    private GameObject igor;

    /// <summary>
    /// reference to Jeve
    /// </summary>
    public GameObject jeve;

    /// <summary>
    /// flag which will be set by the first play to prepare scene that it is running only one time
    /// </summary>
    private bool done = false;

    /// <summary>
    /// a variable
    /// </summary>
    private bool run = true;
    /// <summary>
    /// cutscene script
    /// </summary>
    public override void Script()
    {
        igor = Game.GetIgor();

        if (!run)
        {
            if (!done)
            {
                Append(new ActionSetCutscene(true));

//                Append(new ActionSetKinematic(igor, true));

                Append(new ActionShowMessage("Jeve: Hier unten!"));
                Append(new ActionTranslate(igor, new Vector3(75.80586f, 39.12146f, 134.5844f), 3f));
                Append(new ActionAnimate(igor, "AnimationIndex", 1));
                Sleep(3f);
                Append(new ActionAnimate(igor, "AnimationIndex", 0));

                Sleep(2f);
                Append(new ActionShowMessage("Jeve: Okay, gut gemacht! Jetzt versuchen wir mal ein paar Items einzusammeln."));
                Sleep(1f);
                Append(new ActionAnimate(jeve, "AnimationIndex", 1));
                Append(new ActionRotate(jeve, new Vector3(0, 55f, 0), 0.5f));
                Sleep(0.5f);
                Append(new ActionTranslate(jeve, Vector3.forward * 20, 2f, true));
                Sleep(2f);
                Append(new ActionRotate(jeve, new Vector3(0, 90f, 0), 0.2f));
                Sleep(0.2f);
                Append(new ActionTranslate(jeve, Vector3.forward * 5, 1f, true));
                Sleep(1f);

                Append(new ActionTranslate(jeve, Vector3.forward * 50, 5f, true));
                Append(new ActionSetCutscene(false));
                Sleep(5f);
                Append(new ActionAnimate(jeve, "AnimationIndex", 0));

//                Append(new ActionSetKinematic(igor, false));

                done = true;
            }
        }
        run = false;
    }
}
