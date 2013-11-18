using UnityEngine;
using System.Collections;

/// <summary>
/// a message displayed on the message board
/// </summary>
public class MessageBoardMessage
{
    /// <summary>
    /// parameters controlling the behavior
    /// </summary>
    private readonly float totalMessageWidth = 0.4f; // as factor of Screen.width

    /// <summary>
    /// see above
    /// </summary>
    private readonly float maxTimeInSecondsPerChar = 0.06f;

    /// <summary>
    /// see above
    /// </summary>
    private readonly float fadeInTimeInSeconds = .5f;

    /// <summary>
    /// see above
    /// </summary>
    private readonly float fadeOutTimeInSeconds = 1f;

    /// <summary>
    /// can be omitted
    /// </summary>
    private Texture2D icon;

    /// <summary>
    /// the message
    /// </summary>
    private string message;

    /// <summary>
    /// the contents
    /// </summary>
    private GUIContent guiContent;

    /// <summary>
    /// whether the message is displayed in the middle of the screen
    /// </summary>
    public bool isDialogueMessage = false;

    /// <summary>
    /// used to fade the element in and out and then finally remove it
    /// </summary>
    private float age = 0f;

    /// <summary>
    /// to check whether the message wants to be removed after the current frame
    /// </summary>
    public bool shouldBeRemoved = false;

    /// <summary>
    /// creates a new message, that's what constructors usually do
    /// </summary>
    /// <param name="_message">the message</param>
    /// <param name="_icon">the icon</param>
    /// <param name="_isDialogueMessage">if in center</param>
    public MessageBoardMessage(string _message, Texture2D _icon, bool _isDialogueMessage)
    {
        message = _message;
        icon = _icon;
        isDialogueMessage = _isDialogueMessage;

        guiContent = new GUIContent(message);
    }

    /// <summary>
    /// for internal use
    /// </summary>
    /// <returns>the total message width</returns>
    private float GetTotalMessageWidth()
    {
        return isDialogueMessage ? Mathf.Min(1f, totalMessageWidth * 2f) : totalMessageWidth;
    }

    /// <summary>
    /// for internal use
    /// </summary>
    /// <param name="totalWdt">some width</param>
    /// <returns>the text width</returns>
    private float GetTextWidth(float totalWdt)
    {
        if (icon == null)
        {
            return totalWdt;
        }
        return totalWdt - MessageBoard.GetGUIStyle(isDialogueMessage).lineHeight;
    }

    /// <summary>
    /// returns the height of the element after word-wrapping
    /// </summary>
    /// <returns>the height</returns>
    public float GetHeight()
    {
        return MessageBoard.GetGUIStyle(isDialogueMessage).CalcHeight(guiContent, GetTextWidth(Screen.width * GetTotalMessageWidth()));
    }

    /// <summary>
    /// draws the element with an offset relative to the top edge of the screen
    /// </summary>
    /// <param name="offsetX">the offset</param>
    /// <param name="offsetY">the offset</param>
    public void Draw(float offsetX, float offsetY)
    {
        float totalWdt = Screen.width * GetTotalMessageWidth();
        float totalHgt = GetHeight();
        float textWdt = GetTextWidth(totalWdt);

        // handle fading and removal
        float maxAge = maxTimeInSecondsPerChar * message.Length + 5f;
        age += 1f * Time.deltaTime;

        float alpha = 1.0f;

        if (age < fadeInTimeInSeconds)
        {
            alpha = age / fadeInTimeInSeconds;
        }
        else if (age > maxAge - fadeOutTimeInSeconds)
        {
            alpha = Mathf.Max(0f, maxAge - age) / fadeOutTimeInSeconds;

            if (age > maxAge)
            {
                shouldBeRemoved = true;
            }
        }

        // store alpha for resetting it later
        Color oldGUIColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, alpha);

        // draw GUI
        float x = isDialogueMessage ? (Screen.width - totalWdt) * 0.5f : offsetX + (Screen.width - totalWdt);
        float y = isDialogueMessage ? (Screen.height - totalHgt - 75f) : offsetY;

        // background first
        MessageBoard.DrawMessageBackground((int)x, (int)y, (int)totalWdt, (int)totalHgt);

        if (icon)
        {
            GUI.Box(new Rect(x + textWdt, y, totalWdt - textWdt, MessageBoard.GetGUIStyle(isDialogueMessage).lineHeight), icon, MessageBoard.GetGUIStyle(isDialogueMessage));
        }

        GUI.Box(new Rect(x, y, textWdt, totalHgt), guiContent, MessageBoard.GetGUIStyle(isDialogueMessage));

        // reset alpha
        GUI.color = oldGUIColor;
    }
}
