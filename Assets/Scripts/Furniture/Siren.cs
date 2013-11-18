using UnityEngine;
using System.Collections;

/// <summary>
/// Siren control script
/// </summary>
public class Siren : MonoBehaviour
{
    /// <summary>
    /// List of lights to rotate
    /// </summary>
    public GameObject[] lights;

    /// <summary>
    /// Speed of the rotating lights
    /// </summary>
    public float speed;

    /// <summary>
    /// Used for enabling and disabling the siren
    /// </summary>
    public bool isEnabled;

    /// <summary>
    /// Saves if the lights are enabled or disabled
    /// </summary>
    private bool intEnabled = true;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (!isEnabled)
        {
            if (intEnabled)
            {
                intEnabled = false;
                foreach (GameObject t in lights)
                {
                    t.GetComponent<Light>().enabled = false;
                }
            }
            return;
        }
        else
        {
            if (!intEnabled)
            {
                intEnabled = true;
                foreach (GameObject t in lights)
                {
                    t.GetComponent<Light>().enabled = true;
                }
            }
        }

        if (Game.IsPaused)
        {
            return;
        }

        foreach (GameObject t in lights)
        {
            t.transform.Rotate(Vector3.right * Time.deltaTime * speed, Space.Self);
        }
    }
}
