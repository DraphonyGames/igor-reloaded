using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Shows the credits
/// </summary>
public class Credits : MonoBehaviour
{
    /// <summary>
    /// Coordinates of a pixel
    /// </summary>
    private struct Coords
    {
        /// <summary>
        /// x coordinate
        /// </summary>
        public int x;

        /// <summary>
        /// y coordinate
        /// </summary>
        public int y;
    }

    /// <summary>
    /// internal render size
    /// </summary>
    private const int renderSizeX = 1280;

    /// <summary>
    /// internal render size
    /// </summary>
    private const int renderSizeY = 800;

    /// <summary>
    /// Image used as background
    /// </summary>
    private Texture2D img = null;

    /// <summary>
    /// back texture
    /// </summary>
    public Texture2D blackPixel = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex01IgorChronicles = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex02Design = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex03Design = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex04Proj = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex05Proj = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex06Devel = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex07Devel = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex08Artists = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex09Artists = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex10Alpha = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex11Alpha = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex12Beta = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex13Beta = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex14SpecialThanks = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex15SpecialThanks = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex16SpecialThanks = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex17RobotHarming = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex18Draph = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex19Draph = null;

    /// <summary>
    /// Tex to show
    /// </summary>
    public Texture2D tex20Draph = null;

    /// <summary>
    /// Current image
    /// </summary>
    private int currentImg = 0;

    /// <summary>
    /// Time in the current image
    /// </summary>
    private float currentImgTime = 0;

    /// <summary>
    /// Time for the effect of one image
    /// </summary>
    private float effectTimePerImage = 1.5f;

    /// <summary>
    /// Time for one image
    /// </summary>
    private float timePerImg = 5;

    /// <summary>
    /// Pixel to do (used at various effects)
    /// </summary>
    private List<Coords> pixelToDo = null;

    /// <summary>
    /// Current position
    /// </summary>
    private int pixelPos = 0;

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Start() 
    {
        img = new Texture2D(renderSizeX, renderSizeY);

        for (int x = 0; x < renderSizeX; x++)
        {
            for (int y = 0; y < renderSizeY; y++)
            {
                img.SetPixel(x, y, Color.black);
            }
        }

        img.Apply();
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Update() 
    {
        // Speed up some animations
        float realTimePerImg = timePerImg;

        switch (currentImg)
        {
            case 0: EffectPixelFlip(tex01IgorChronicles);
                realTimePerImg *= 2f;
                break;
            case 1: EffectPixelFlip(tex02Design);
                realTimePerImg *= 1f;
                break;
            case 2: EffectPixelFlip(tex03Design);
                realTimePerImg *= 0.5f;
                break;
            case 3: EffectPixelFlip(tex04Proj);
                realTimePerImg *= 0.5f;
                break;
            case 4: EffectPixelFlip(tex05Proj);
                realTimePerImg *= 0.5f;
                break;
            case 5: EffectPixelFlip(tex06Devel);
                realTimePerImg *= 0.5f;
                break;
            case 6: EffectPixelFlip(tex07Devel);
                realTimePerImg *= 1f;
                break;
            case 7: EffectPixelFlip(tex08Artists);
                realTimePerImg *= 1f;
                break;
            case 8: EffectPixelFlip(tex09Artists);
                realTimePerImg *= 2f;
                break;
            case 9: EffectPixelFlip(tex10Alpha);
                realTimePerImg *= 0.5f;
                break;
            case 10: EffectPixelFlip(tex11Alpha);
                realTimePerImg *= 0.5f;
                break;
            case 11:
                EndScene();
                break;
        }

        currentImgTime += Time.deltaTime;
        if (currentImgTime > realTimePerImg)
        {
            currentImgTime = 0;
            currentImg++;
        }

        /* Allow skipping the whole scene*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndScene();
        }
    }

    /// <summary>
    /// Return to e.g. main menu
    /// </summary>
    private void EndScene()
    {
        Application.LoadLevel("Startscreen");
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackPixel, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), img, ScaleMode.ScaleToFit);
    }

    /// <summary>
    /// Generate PixelToDo
    /// </summary>
    private void GeneratePixelToDo()
    {
        /* We don't need to redo this every time, nobody will see it */
        if (pixelToDo == null)
        {
            pixelToDo = new List<Coords>();

            for (int x_ = 0; x_ < renderSizeX; x_++)
            {
                for (int y_ = 0; y_ < renderSizeY; y_++)
                {
                    pixelToDo.Add(new Coords { x = x_, y = y_ });
                }
            }

            /* 'Randomize' */
            Coords tmp;
            for (int max = pixelToDo.Count - 1; max > 0; max--)
            {
                int current = (int)UnityEngine.Random.Range(0, max);
                tmp = pixelToDo[current];
                pixelToDo[current] = pixelToDo[max];
                pixelToDo[max] = tmp;
            }
        }

        pixelPos = 0;
    }

    /// <summary>
    /// Flips the pixel time after time
    /// </summary>
    /// <param name="otherImg">credit image to flip to</param>
    private void EffectPixelFlip(Texture2D otherImg)
    {
        if (otherImg == null)
        {
            Debug.LogWarning("Credits: Image not set!");
            return;
        }

        if (currentImgTime == 0)
        {
            GeneratePixelToDo();
        }

        if (pixelToDo == null || pixelToDo.Count == 0) // Test for null: some weird Unity bug when running it from the editor..
        {
            return;
        }

        /* Calculate amount of pixel to flip */
        int amtToFlip = (int)(((renderSizeX * renderSizeY) / effectTimePerImage) * Time.deltaTime);

        for (int i = 0; i < amtToFlip && pixelPos < pixelToDo.Count; i++)
        {
            Coords c = pixelToDo[pixelPos++];

            Coords innerStart = new Coords { x = (renderSizeX - otherImg.width) / 2, y = (renderSizeY - otherImg.height) / 2 };

            // When we are outside: just color it black
            if (c.x < innerStart.x || c.y < innerStart.y || c.x > innerStart.x + otherImg.width || c.y > innerStart.y + otherImg.height)
            {
                img.SetPixel(c.x, c.y, Color.black);
            }
            else
            {
                img.SetPixel(c.x, c.y, otherImg.GetPixel(c.x - innerStart.x, c.y - innerStart.y));
            }
        }

        img.Apply();
    }
}
