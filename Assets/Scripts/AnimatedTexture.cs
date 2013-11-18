using UnityEngine;
using System.Collections;

/// <summary>
/// animate the main texture of the first material by animate offset and scale of the texture
/// </summary>
public class AnimatedTexture : MonoBehaviour {
    /// <summary>
    /// define the end of the scale animation 
    /// </summary>
    public float maxTextureSize = 1;

    /// <summary>
    /// define the start of the animation scale
    /// </summary>
    public float minTextureSize = 1;

    /// <summary>
    /// define the end of the offset animation for x axe
    /// </summary>
    public float maxTextureOffsetX = 1;

    /// <summary>
    /// define the start of the offset animation for x axe
    /// </summary>
    public float minTextureOffsetX = 0;

    /// <summary>
    /// define the end of the offset animation for Y axe
    /// </summary>
    public float maxTextureOffsetY = 1;

    /// <summary>
    /// define the start of the offset animation for Y axe
    /// </summary>
    public float minTextureOffsetY = 0;

    /// <summary>
    /// define the scale animation speed 
    /// </summary>
    public float textureScaleSpeed = 1;

    /// <summary>
    /// define the offset animation speed
    /// </summary>
    public float textureOffsetSpeed = 1;

    /// <summary>
    /// current x offset
    /// </summary>
    private float offsetX = 0;

    /// <summary>
    /// current y offset
    /// </summary>
    private float offsetY = 0;

    /// <summary>
    /// current scale
    /// </summary>
    private float scale = 0;

    /// <summary>
    /// start method
    /// </summary>
    private void Start()
    {
        offsetX = minTextureOffsetX;
        offsetY = minTextureOffsetY;
        scale = minTextureSize;
    }

    /// <summary>
    /// update method
    /// </summary>
    private void Update()
    {
        // Calculate offset for this frame
        offsetY += Time.deltaTime * textureOffsetSpeed;
        offsetX += Time.deltaTime * textureOffsetSpeed;
        scale += Time.deltaTime * textureScaleSpeed;

        // Loop scaling texture
        if (scale > maxTextureSize)
        {
            scale = minTextureSize;
        }

        // Loop moving texture ...
        if (offsetY > maxTextureOffsetY)
        {
            offsetY = minTextureOffsetY;
        }
        // ...
        if (offsetX > maxTextureOffsetX)
        {
            offsetX = minTextureOffsetX;
        }

        // Set new offset and scale

        foreach (Material m in renderer.materials)
        {
            m.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            m.SetTextureScale("_MainTex", new Vector2(scale, scale));
        }
    }
}
