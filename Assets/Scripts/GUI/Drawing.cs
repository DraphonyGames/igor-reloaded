using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class for drawing stuff. Currently only DrawLine() possible
/// </summary>
public static class Drawing
{
    /// <summary>
    /// 1x1 pixel used for the line
    /// </summary>
    private static Texture2D lineTexture = null;

    /// <summary>
    /// Reusable rectangle for drawing later on
    /// </summary>
    private static Rect onexoneRect = new Rect(0, 0, 1, 1);

    /// <summary>
    /// Draw a line
    /// </summary>
    /// <param name="pointA">Start point</param>
    /// <param name="pointB">End point</param>
    /// <param name="drawRect">Area to draw in - set to (0, 0, 0, 0) to disable clipping</param>
    /// <param name="color">color to paint the line in</param>
    /// <param name="width">width of the line</param>
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Rect drawRect, Color color, float width)
    {
        /* Allows setting the Rect to (0,0,0,0) to skip clipping */
        if (drawRect.x != 0 || drawRect.y != 0 || drawRect.width != 0 || drawRect.height != 0)
        {
            if (InRect(pointA, drawRect))
            {
                if (InRect(pointB, drawRect))
                {
                    /* Everthings ok, don't do anything */
                }
                else
                {
                    /* We have to move pointB, so we 'go' from A to B and get the first collision with the box */
                    pointB = GetCollisionWithBoarder(pointA, pointB, drawRect);
                }
            }
            else
            {
                if (InRect(pointB, drawRect))
                {
                    /* We only need to switch A and B.. recursive call, since I can't jump upwards with goto... srsly C#?! */
                    DrawLine(pointB, pointA, drawRect, color, width);
                    return;
                }
                else
                {
                    /* both points are outside the rectangle 
                     * 
                     * if we get a (-1, -1), then we don't cross the rectangle */

                    pointA = GetCollisionWithBoarder(pointA, pointB, drawRect);

                    if (pointA.x == -1 || pointA.y == -1)
                    {
                        return;
                    }

                    pointB = GetCollisionWithBoarder(pointB, pointA, drawRect);

                    if (pointB.x == -1 || pointB.y == -1)
                    {
                        return;
                    }

                    /* Check if both points are inside the rectangle, have some bigger boarder for floating point errors */
                    Rect histDrawRect = new Rect(drawRect.x - 0.2f, drawRect.y - 0.2f, drawRect.width + 0.4f, drawRect.height + 0.4f);
                    if (!InRect(pointA, histDrawRect) || !InRect(pointB, histDrawRect))
                    {
                        return;
                    }
                }
            }
        }

        /* We create our 1x1 pixel ourself */
        if (!lineTexture)
        {
            lineTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            lineTexture.SetPixel(0, 0, Color.white);
            lineTexture.Apply();
        }

        Matrix4x4 mat = Matrix4x4.TRS(pointA + Vector2.up * (-0.5f) * width, Quaternion.FromToRotation(Vector2.right, pointB - pointA), new Vector3((pointB - pointA).magnitude, width, 1));

        Color oldColor = GUI.color;
        GUI.color = color;

        GL.PushMatrix();
        GL.MultMatrix(mat);
        //Graphics.DrawTexture(onexoneRect, lineTexture, onexoneRect, 0, 0, 0, 0, color); // Won't work for scrollbars in a good way
        GUI.DrawTexture(onexoneRect, lineTexture); // Somehow GUI and GL share the matrix.. ? Either way, this works for scroll bars
        GL.PopMatrix();

        GUI.color = oldColor;
    }

    /// <summary>
    /// Returns the first collision of the line going from "from" to "to" or (-1, -1) if no collision happens
    /// </summary>
    /// <param name="from">Line starting point</param>
    /// <param name="to">Line ending point</param>
    /// <param name="rect">Rectangle to calculate collision with</param>
    /// <returns>Returns the first collision of the line going from "from" to "to" or (-1, -1) if no collision happens</returns>
    public static Vector2 GetCollisionWithBoarder(Vector2 from, Vector2 to, Rect rect)
    {
        /* Using the following vectors:
         * o-------->
         * |         ^
         * |         |
         * v         |
         *  <--------o
         */

        float right = (new Matrix2x2(to.x - from.x, 1, to.y - from.y, 0)).Determinant();
        float down = (new Matrix2x2(to.x - from.x, 0, to.y - from.y, 1)).Determinant();
        float left = (new Matrix2x2(to.x - from.x, -1, to.y - from.y, 0)).Determinant();
        float up = (new Matrix2x2(to.x - from.x, 0, to.y - from.y, -1)).Determinant();

        /* matrices for cramers rule, right side inserted */
        float cramUpRightRight = (new Matrix2x2(rect.x - from.x, 1, rect.y - from.y, 0)).Determinant();
        float cramUpRightDown = (new Matrix2x2(rect.x - from.x, 0, rect.y - from.y, 1)).Determinant();
        float cramDownLeftLeft = (new Matrix2x2(rect.x + rect.width - from.x, -1, rect.y + rect.height - from.y, 0)).Determinant();
        float cramDownLeftUp = (new Matrix2x2(rect.x + rect.width - from.x, 0, rect.y + rect.height - from.y, -1)).Determinant();

        List<float> usuableSolutions = new List<float>();

        if (right != 0)
        {
            usuableSolutions.Add(cramUpRightRight / right);
        }

        if (down != 0)
        {
            usuableSolutions.Add(cramUpRightDown / down);
        }

        if (left != 0)
        {
            usuableSolutions.Add(cramDownLeftLeft / left);
        }

        if (up != 0)
        {
            usuableSolutions.Add(cramDownLeftUp / up);
        }

        float result = ReturnMinimalPositive(usuableSolutions);

        if (result == -1)
        {
            return new Vector2(-1, -1);
        }

        return from + ((to - from) * result);
    }

    /// <summary>
    /// Returns whether the point is in the rectangle or not
    /// </summary>
    /// <param name="point">the point</param>
    /// <param name="rect">the area</param>
    /// <returns>the result</returns>
    public static bool InRect(Vector2 point, Rect rect)
    {
        return rect.xMin <= point.x && point.x <= rect.xMax && rect.yMin <= point.y && point.y <= rect.yMax;
    }

    /// <summary>
    /// Returns smallest positive (>=0) value. If none are available, return -1
    /// </summary>
    /// <param name="values">the values</param>
    /// <returns>Smallest positive (>=0) value or -1 if failure</returns>
    public static float ReturnMinimalPositive(IList<float> values)
    {
        if (values.Count == 0)
        {
            goto error;
        }

        float min = float.MaxValue;

        foreach (float value in values)
        {
            if (min > value && value >= 0)
            {
                min = value;
            }
        }

        // No valid solution was found
        if (min == float.MaxValue)
        {
            goto error;
        }

        return min;


    error:
        return -1;
    }
}