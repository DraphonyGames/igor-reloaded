using UnityEngine;
using System.Collections;

/// <summary>
/// Makes a ceiling lamp flickering
/// </summary>
public class BrokenCeilingLamp : MonoBehaviour
{
    /// <summary>
    /// on = on, off = currently flickering
    /// </summary>
    private bool on = true;

    /// <summary>
    /// Time since last flickering
    /// </summary>
    private float lastFlicker = 0;

    /// <summary>
    /// Max time until a flicker happens.
    /// </summary>
    public float maxTimeToFlicker = 7;

    /// <summary>
    /// Max flicker length
    /// </summary>
    public float maxFlickerLength = 1.5f;

    /// <summary>
    /// Material for display
    /// </summary>
    public Material onMaterial;

    /// <summary>
    /// Material for display
    /// </summary>
    public Material offMaterial;
    /// <summary>
    /// some variable
    /// </summary>
    private float nextChange = 0.5f;

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Update()
    {
        if (on)
        {
            if ((Time.time - lastFlicker) / maxTimeToFlicker > nextChange)
            {
                on = !on;
                renderer.material = offMaterial;
                lastFlicker = Time.time;
                nextChange = Random.value;

                foreach (Light l in GetComponentsInChildren<Light>())
                {
                    l.enabled = false;
                }
            }
        }
        else
        {
            if ((Time.time - lastFlicker) / maxFlickerLength > nextChange)
            {
                on = !on;
                renderer.material = onMaterial;
                lastFlicker = Time.time;
                nextChange = Random.value;

                foreach (Light l in GetComponentsInChildren<Light>())
                {
                    l.enabled = true;
                }
            }
        }
    }
}
