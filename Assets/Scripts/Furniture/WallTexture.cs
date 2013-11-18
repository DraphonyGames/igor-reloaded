using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically scales a texture on a wall.
/// </summary>
public class WallTexture : MonoBehaviour
{
    /// <summary>
    /// The global scaling factor for both directions (x and y).
    /// </summary>
    public float scaleFactor = 0.1f;

    /// <summary>
    /// Set wall texture scaling according to cube scaling.
    /// </summary>
    protected void Start()
    {
        if (!renderer)
        {
            return;
        }

        Vector3 extents = transform.rotation * renderer.bounds.extents;
        renderer.material.SetTextureScale("_MainTex", new Vector2(extents.z * scaleFactor, extents.y * scaleFactor));
        renderer.material.SetTextureScale("_BumpMap", new Vector2(extents.z * scaleFactor, extents.y * scaleFactor));

        Vector3 localScale = renderer.transform.localScale;
        localScale.x *= Random.Range(0.98f, 1f);
        renderer.transform.localScale = localScale;
    }
}
