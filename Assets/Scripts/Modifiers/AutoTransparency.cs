using UnityEngine;
using System.Collections;

/// <summary>
/// sets transparency
/// </summary>
public class AutoTransparency : MonoBehaviour
{
    /// <summary>
    /// Save old shader of object.
    /// </summary>
    private Shader oldShader = null;

    /// <summary>
    /// Save old color of object
    /// </summary>
    private Color oldColor;

    /// <summary>
    /// Current transparency.
    /// </summary>
    private float transparency = 1f;

    /// <summary>
    /// The transparency the object should have.
    /// </summary>
    private const float WantedTransparency = 0.5f;

    /// <summary>
    /// Time in seconds until which 100% visibility is restored.
    /// </summary>
    private const float FalloffTime = 0.5f;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Start()
    {
        oldShader = Shader.Find("Diffuse");
    }

    /// <summary>
    /// Makes the object transparent.
    /// </summary>
    public void BeTransparent()
    {
        transparency = WantedTransparency;

        if (oldShader == null)
        {
            oldShader = renderer.material.shader;
            oldColor = renderer.material.color;
            renderer.material.shader = Shader.Find("Transparent/Diffuse");
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (transparency < 1f)
        {
            Color c = renderer.material.color;
            c.a = transparency;
            renderer.material.color = c;
        }
        else
        {
            renderer.material.shader = oldShader;
            renderer.material.color = oldColor;

            Destroy(this);
        }

        transparency += ((1f - WantedTransparency) * Time.deltaTime) / FalloffTime;
    }
}
