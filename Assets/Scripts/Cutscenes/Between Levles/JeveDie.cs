using UnityEngine;
using System.Collections;

/// <summary>
/// the cutscene of Jeve's HORRIBLE DEATH
/// </summary>
public class JeveDie : MovementManager
{
    /// <summary>
    /// spark which will spawn on the console
    /// </summary>
    public GameObject spark;

    /// <summary>
    /// hit which will spawn on the console when igor attack it
    /// </summary>
    public GameObject hitEffect;

    /// <summary>
    /// reference to explosion
    /// </summary>
    public GameObject jeveDead;

    /// <summary>
    /// reference to explosion
    /// </summary>
    public GameObject explosion;

    /// <summary>
    /// reference to camera
    /// </summary>
    public GameObject cam;

    /// <summary>
    /// reference to siren
    /// </summary>
    public GameObject siren;

    /// <summary>
    /// reference to Fatal Error text
    /// </summary>
    public GameObject enemyText4;

    /// <summary>
    /// reference to Fatal Error text
    /// </summary>
    public GameObject enemyText3;

    /// <summary>
    /// reference to Fatal Error text
    /// </summary>
    public GameObject enemyText2;

    /// <summary>
    /// reference to Fatal Error text
    /// </summary>
    public GameObject enemyText1;

    /// <summary>
    /// reference to Fatal Error text
    /// </summary>
    public GameObject fatalError;

    /// <summary>
    /// reference to booting text
    /// </summary>
    public GameObject booting;

    /// <summary>
    /// error screen 
    /// </summary>
    public GameObject errorScreen;

    /// <summary>
    /// reference to Diagnostic Robot
    /// </summary>
    public GameObject robot;

    /// <summary>
    /// reference to Gill
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
        Append(new ActionAnimate(igor, "AnimationIndex", 0));

        Append(new ActionAnimate(jeve, "AnimationIndex", 2));

        Append(new ActionShowMessage("Jeve: Okay Igor, es wird Zeit, dass du deinen Schöpfer kennenlernst.\nFreust du dich schon?"));
        Sleep(5f);
        Append(new ActionShowMessage("IGOR: Ja!! Juhu, endlich lerne ich meinen Schöpfer kennen, du hast mir schon soviel von ihm erzählt."));
        Append(new ActionAnimate(igor, "AnimationIndex", -3));
        Sleep(1f);

        Sleep(2f);
        Append(new ActionRotate(cam, new Vector3(0, 335, 0), 5f));
        Append(new ActionRotate(jeve, new Vector3(0, 90, 0), 0.1f));
        Append(new ActionTranslate(jeve, Vector3.forward * 29, 5f, true));

        Append(new ActionAnimate(jeve, "AnimationIndex", 1));

        Sleep(2f);

        Append(new ActionRotate(jeve, new Vector3(0, 70, 0), 0.3f));

        Sleep(1f);

        Sleep(2f);
        Append(new ActionAnimate(jeve, "AnimationIndex", 0));
        Append(new ActionSetRenderer(errorScreen.GetComponent<Renderer>(), true));
        Append(new ActionSetRenderer(fatalError.GetComponent<Renderer>(), true));
        Append(new ActionAnimate(igor, "AnimationIndex", -1));
        Sleep(5f);
        Append(new ActionInstantiate(jeveDead, new Vector3(338.8253f, 0.5f, 366.9187f), Quaternion.Euler(new Vector3(0, 0, 0f))));
        Append(new ActionRemove(jeve));
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Append(new ActionSetRenderer(fatalError.GetComponent<Renderer>(), false));
        Append(new ActionSetRenderer(booting.GetComponent<Renderer>(), true));
        Sleep(4.9f);
        Append(new ActionAnimate(igor, "AnimationIndex", -2));
        Sleep(0.1f);
        Append(new ActionSetRenderer(errorScreen.GetComponent<Renderer>(), false));
        Append(new ActionSetRenderer(booting.GetComponent<Renderer>(), false));
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(2.5f);
        Append(new ActionRotate(igor, new Vector3(0, 45, 0), 0.1f));

        Sleep(0.4f);
        Append(new ActionShowMessage("IGOR: Jeve?"));
        Sleep(0.4f);
        Append(new ActionAnimate(igor, "AnimationIndex", 1));
        Append(new ActionTranslate(igor, new Vector3(336.5f, 2.5f, 366f), 1f));

        Sleep(0.9f);
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(0.1f);
        Append(new ActionShowMessage("IGOR: Nein!! Vedammt!"));
        Append(new ActionAnimate(igor, "AnimationIndex", -6));
        Sleep(0.5f);

        Append(new ActionTranslate(robot, Vector3.forward * 20, 8f, true));

        Append(new ActionRotate(igor, new Vector3(0, 270, 0), 2f));
        Sleep(4f);
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(4f);
        Append(new ActionSetRenderer(enemyText1.GetComponent<Renderer>(), true));
        Sleep(1.2f);
        Append(new ActionSetRenderer(enemyText2.GetComponent<Renderer>(), true));
        Sleep(1.2f);
        Append(new ActionSetRenderer(enemyText3.GetComponent<Renderer>(), true));
        Sleep(1.8f);
        Append(new ActionSetRenderer(enemyText4.GetComponent<Renderer>(), true));
        Sleep(1f);

        Append(new ActionSetRenderer(enemyText4.GetComponent<Renderer>(), false));

        Append(new ActionSetRenderer(enemyText3.GetComponent<Renderer>(), false));

        Append(new ActionSetRenderer(enemyText2.GetComponent<Renderer>(), false));

        Append(new ActionSetRenderer(enemyText1.GetComponent<Renderer>(), false));

        Append(new ActionInstantiate(siren, new Vector3(291f, 19.8f, 372.4f), Quaternion.Euler(0, 90, 0)));
        Append(new ActionScale(siren.transform, new Vector3(0.1f, 0.1f, 0.1f), 0.01f));
        Append(new ActionShowMessage("ANLAGE: ALARM! Notfall: Fehlfunktion einer Einheit. Alle Ausgänge werden verriegelt."));
        Sleep(0.5f);
        Append(new ActionTranslate(igor, new Vector3(303.0444f, 2.6f, 372.132f), 0.5f));
        Sleep(0.1f);
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(0.3f);
        Append(new ActionAnimate(igor, "AnimationIndex", 2));
        Sleep(1f);
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Append(new ActionRemove(robot));

        Append(new ActionInstantiate(explosion, new Vector3(303.0444f, 1f, 372.132f), Quaternion.Euler(0, 0, 0)));
        Append(new ActionAnimate(igor, "AnimationIndex", -6));
        Sleep(1f);
        Append(new ActionShowMessage("IGOR: Jeve! Was mach ich jetzt nur!"));

        Sleep(5f);
        Append(new ActionRotate(cam, new Vector3(0, 220, 0), 3f));
        Append(new ActionRotate(igor, new Vector3(0, -180, 0), 0.1f));
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(0.1f);
        Append(new ActionAnimate(igor, "AnimationIndex", 1));

        Append(new ActionTranslate(igor, new Vector3(314.8863f, 6f, 316f), 2f, false));
        Sleep(2f);
        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(0.1f);
        Append(new ActionAnimate(igor, "AnimationIndex", 2));
        Append(new ActionShowMessage("IGOR: [ Alarm abschalten!! ]"));
        Sleep(1.2f);
        Append(new ActionInstantiate(hitEffect, new Vector3(316f, 4f, 318f), Quaternion.Euler(0, 0, 0)));
        Sleep(2f);
        Append(new ActionInstantiate(hitEffect, new Vector3(316f, 4f, 318f), Quaternion.Euler(0, 0, 0)));
        Sleep(2f);
        Append(new ActionShowMessage("Anlage: ERROR.."));
        Append(new ActionInstantiate(explosion, new Vector3(316f, 4f, 318f), Quaternion.Euler(0, 0, 0)));
        Append(new ActionInstantiate(spark, new Vector3(311f, 4f, 312f), Quaternion.Euler(0, 0, 0)));
        Append(new ActionAnimate(igor, "AnimationIndex", 0));

        Sleep(2f);

        Append(new ActionAnimate(igor, "AnimationIndex", 0));
        Sleep(0.1f);
        Append(new ActionAnimate(igor, "AnimationIndex", 1));
        Append(new ActionRotate(igor, new Vector3(0, 0, 0), 0.1f));

        Append(new ActionTranslate(igor, new Vector3(298f, 0.5f, 370), 2f, false));
        Append(new ActionRotate(cam, new Vector3(0, 300, 0), 3f));
        Append(new ActionShowMessage("IGOR: [Ich versuche zum Hauptkontrollraum zu kommen, um die Verriegelung abzuschalten]"));

        Sleep(2f);
        Append(new ActionRotate(igor, new Vector3(0, 270, 0), 0.1f));
        Append(new ActionTranslate(igor, new Vector3(292f, 0.5f, 370), 2f, false));
        Sleep(0.5f);
        Append(new ActionSetCutscene(false));
        Append(new ActionLoadScene("JevesFloor"));
        
        IsRunning = true;
    }
}
