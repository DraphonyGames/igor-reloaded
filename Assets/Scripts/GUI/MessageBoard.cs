using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// the message board system
/// </summary>
public class MessageBoard : MonoBehaviour
{
    #region StaticInterface

    /// <summary>
    /// for internal use only
    /// </summary>
    private static MessageBoard msgBoardInstance;

    /// <summary>
    /// internal. To retrieve and/or create the message board instance
    /// </summary>
    /// <returns>returns the only ring</returns>
    private static MessageBoard GetMessageBoardInstance()
    {
        if (MessageBoard.msgBoardInstance == null)
        {
            MessageBoard.msgBoardInstance = (MessageBoard)((GameObject)Instantiate(Resources.Load("MessageBoardPrefab"))).GetComponent(typeof(MessageBoard));
        }
        return MessageBoard.msgBoardInstance;
    }

    /// <summary>
    /// adds a message to the message board that will be shown when possible
    /// </summary>
    /// <param name="message">some message</param>
    /// <param name="icon">some image</param>
    /// <param name="dialogueMessage">if in center of screen</param>
    public static void AddMessage(string message, Texture2D icon = null, bool dialogueMessage = false)
    {
        GetMessageBoardInstance().Add(message, icon, dialogueMessage);
    }

    /// <summary>
    /// draws a message background into the GUI.
    /// Note that the actual texture might extent over the original rectangle (which will be the center)
    /// </summary>
    /// <param name="x">coordinate a</param>
    /// <param name="y">coordinate b</param>
    /// <param name="wdt">some width</param>
    /// <param name="hgt">some height</param>
    public static void DrawMessageBackground(int x, int y, int wdt, int hgt)
    {
        GetMessageBoardInstance().DrawMsgBackground(x, y, wdt, hgt);
    }

    /// <summary>
    /// wrapper of something
    /// </summary>
    /// <param name="x">some coordinate</param>
    /// <param name="y">some coordinate</param>
    /// <param name="wdt">some width</param>
    /// <param name="hgt">some height</param>
    public static void DrawMessageBackground(float x, float y, float wdt, float hgt)
    {
        DrawMessageBackground((int)x, (int)y, (int)wdt, (int)hgt);
    }

    /// <summary>
    /// draws a message background into the GUI.
    /// Note that the actual texture might extent over the original rectangle (which will be the center)
    /// </summary>
    /// <param name="r">Rectangle to draw</param>
    public static void DrawMessageBackground(Rect r)
    {
        GetMessageBoardInstance().DrawMsgBackground((int)r.x, (int)r.y, (int)r.width, (int)r.height);
    }

    #endregion

    /// <summary>
    /// GUI style for the normal messages
    /// </summary>
    private GUIStyle guiStyle = null;

    /// <summary>
    /// GUI style for the dialogue messages
    /// </summary>
    private GUIStyle guiStyleDialogue = null;

    /// <summary>
    /// the style used for the messages
    /// </summary>
    /// <param name="forDialogues">whether to return the style for dialogue boxes</param>
    /// <returns>the style for boxes</returns>
    public static GUIStyle GetGUIStyle(bool forDialogues = false)
    {
        MessageBoard instance = GetMessageBoardInstance();

        GUIStyle style = forDialogues ? instance.guiStyleDialogue : instance.guiStyle;

        if (style == null)
        {
            style = new GUIStyle();
            style.wordWrap = true;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.0f);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = forDialogues ? 20 : 10;

            if (forDialogues)
            {
                instance.guiStyleDialogue = style;
            }
            else
            {
                instance.guiStyle = style;
            }
        }

        return style;

    }

    /// <summary>
    /// maximum displayed height of several messages together as factor of Screen.height
    /// </summary>
    private readonly float maxHeight = 0.5f;

    /// <summary>
    /// margin between two displayed messages
    /// </summary>
    private readonly float margin = 35f;

    /// <summary>
    /// adds a margin on the left side of the messages (f.e. for the HP bars)
    /// </summary>
    private readonly float marginLeft = -10f;

    /// <summary>
    /// list of messages
    /// </summary>
    private List<MessageBoardMessage> messages = new List<MessageBoardMessage>();

    /// <summary>
    /// width of the border of the message backgrounds' texture
    /// </summary>
    private readonly int borderWidth = 19;

    /// <summary>
    /// for internal use. Called by AddMessage
    /// </summary>
    /// <param name="message">message to add</param>
    /// <param name="icon">some image</param>
    /// <param name="dialogueMessage">if in center</param>
    private void Add(string message, Texture2D icon, bool dialogueMessage)
    {
        messages.Add(new MessageBoardMessage(message, icon, dialogueMessage));
    }

    /// <summary>
    /// Unity OnGUI
    /// </summary>
    private void OnGUI()
    {
        // closer to the camera than the black cutscene bars
        GUI.depth = -5;

        float currentOffset = margin * 0.5f;
        bool drewOneDialogueMessage = false;

        List<MessageBoardMessage> toRemove = new List<MessageBoardMessage>();

        foreach (MessageBoardMessage msg in messages)
        {
            if (msg.isDialogueMessage)
            {
                if (drewOneDialogueMessage)
                {
                    continue;
                }
                drewOneDialogueMessage = true;
            }
            else
            {
                if (currentOffset > Screen.height * maxHeight)
                {
                    continue;
                }
                currentOffset += msg.GetHeight() + margin;
            }

            msg.Draw(marginLeft, currentOffset);

            if (msg.shouldBeRemoved)
            {
                toRemove.Add(msg);
            }
        }

        foreach (MessageBoardMessage msg in toRemove)
        {
            messages.Remove(msg);
        }
    }

    #region BackgroundDrawing

    /// <summary>
    /// contains images
    /// </summary>
    private Texture2D[] backgroundTextureParts = null;

    /// <summary>
    /// list of sensible names
    /// </summary>
    private enum BackgroundTexturePartIndices
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    /// <summary>
    /// creates the textures used to draw the background when called for the very first time
    /// </summary>
    private void GenerateMessageBackgroundTexture()
    {
        // already loaded?
        if (backgroundTextureParts != null)
        {
            return;
        }
        backgroundTextureParts = new Texture2D[9];


        Texture2D mainTex = (Texture2D)Resources.Load("MessageBackgroundTexture");

        CopyTexture(mainTex, BackgroundTexturePartIndices.BottomLeft, 0, 0, borderWidth, borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.Bottom, borderWidth, 0, mainTex.width - 2 * borderWidth, borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.BottomRight, mainTex.width - borderWidth, 0, borderWidth, borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.Left, 0, borderWidth, borderWidth, mainTex.height - 2 * borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.Center, borderWidth, borderWidth, mainTex.width - 2 * borderWidth, mainTex.height - 2 * borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.Right, mainTex.width - borderWidth, borderWidth, borderWidth, mainTex.height - 2 * borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.TopLeft, 0, mainTex.height - borderWidth, borderWidth, borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.Top, borderWidth, mainTex.height - borderWidth, mainTex.width - 2 * borderWidth, borderWidth);
        CopyTexture(mainTex, BackgroundTexturePartIndices.TopRight, mainTex.width - borderWidth, mainTex.height - borderWidth, borderWidth, borderWidth);
    }

    /// <summary>
    /// creates a new texture from a part of a bigger texture
    /// </summary>
    /// <param name="from">some image</param>
    /// <param name="toIndex">some target</param>
    /// <param name="fromX">some source</param>
    /// <param name="fromY">more source</param>
    /// <param name="w">width, the</param>
    /// <param name="h">height, a random</param>
    private void CopyTexture(Texture2D from, BackgroundTexturePartIndices toIndex, int fromX, int fromY, int w, int h)
    {
        Texture2D newTex = new Texture2D(w, h, TextureFormat.ARGB32, false);
        newTex.wrapMode = TextureWrapMode.Repeat;

        backgroundTextureParts[(int)toIndex] = newTex;

        for (int x = fromX; x < fromX + w; ++x)
        {
            for (int y = fromY; y < fromY + h; ++y)
            {
                newTex.SetPixel(x - fromX, y - fromY, from.GetPixel(x, y));
            }
        }
        newTex.Apply();
    }

    /// <summary>
    /// will draw the background of a message - however, the background might extend over the original rectangle
    /// </summary>
    /// <param name="x">some coordinate</param>
    /// <param name="y">some coordinate</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    public void DrawMsgBackground(int x, int y, int wdt, int hgt)
    {
        // generate texture if not already done
        GenerateMessageBackgroundTexture();
        int borderWidth = 10;
        int standardMargin = 5;
        x -= borderWidth + standardMargin;
        y -= borderWidth + standardMargin;
        wdt += 2 * (borderWidth + standardMargin);
        hgt += 2 * (borderWidth + standardMargin);

        DrawTexture(BackgroundTexturePartIndices.TopLeft, x, y, borderWidth, borderWidth);
        DrawTexture(BackgroundTexturePartIndices.Top, x + borderWidth, y, wdt - 2 * borderWidth, borderWidth);
        DrawTexture(BackgroundTexturePartIndices.TopRight, x + wdt - borderWidth, y, borderWidth, borderWidth);
        DrawTexture(BackgroundTexturePartIndices.Left, x, y + borderWidth, borderWidth, hgt - 2 * borderWidth);
        DrawTexture(BackgroundTexturePartIndices.Center, x + borderWidth, y + borderWidth, wdt - 2 * borderWidth, hgt - 2 * borderWidth);
        DrawTexture(BackgroundTexturePartIndices.Right, x + wdt - borderWidth, y + borderWidth, borderWidth, hgt - 2 * borderWidth);
        DrawTexture(BackgroundTexturePartIndices.BottomLeft, x, y + hgt - borderWidth, borderWidth, borderWidth);
        DrawTexture(BackgroundTexturePartIndices.Bottom, x + borderWidth, y + hgt - borderWidth, wdt - 2 * borderWidth, borderWidth);
        DrawTexture(BackgroundTexturePartIndices.BottomRight, x + wdt - borderWidth, y + hgt - borderWidth, borderWidth, borderWidth);
    }

    /// <summary>
    /// draws a certain texture 
    /// </summary>
    /// <param name="index">index of texture to draw</param>
    /// <param name="x">some coordinate</param>
    /// <param name="y">some coordinate</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    private void DrawTexture(BackgroundTexturePartIndices index, int x, int y, int wdt, int hgt)
    {
        GUI.DrawTexture(new Rect(x, y, wdt, hgt), backgroundTextureParts[(int)index]);
    }

    #endregion
}
