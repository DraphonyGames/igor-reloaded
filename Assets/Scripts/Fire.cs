using UnityEngine;
using System.Collections;

/// <summary>
/// a fire class
/// </summary>
public class Fire : MonoBehaviour
{
    /// <summary>
    /// The light of the fire
    /// </summary>
    public Light lightSource;

    /// <summary>
    /// Minimal intensity of the light
    /// </summary>
    public float minIntensity;

    /// <summary>
    /// Maximal intensity of the light
    /// </summary>
    public float maxIntensity;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        lightSource.intensity = Random.Range(0.5f, 1.5f);
    }
}
