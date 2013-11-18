using System;

/// <summary>
/// 2x2 matrix
/// </summary>
public struct Matrix2x2
{
    /// <summary>
    /// all 4 values
    /// </summary>
    public float[,] values;

    /// <summary>
    /// 2x2 matrix
    /// </summary>
    /// <param name="m00">00 parameter</param>
    /// <param name="m01">01 parameter</param>
    /// <param name="m10">10 parameter</param>
    /// <param name="m11">11 parameter</param>
    public Matrix2x2(float m00, float m01, float m10, float m11)
    {
        values = new float[2, 2];

        values[0, 0] = m00;
        values[0, 1] = m01;
        values[1, 0] = m10;
        values[1, 1] = m11;
    }

    /// <summary>
    /// returns the determinant of the matrix
    /// </summary>
    /// <returns>the determinant of the matrix</returns>
    public float Determinant()
    {
        return values[0, 0] * values[1, 1] - values[0, 1] * values[1, 0];
    }
}
