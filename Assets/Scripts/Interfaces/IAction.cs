using UnityEngine;
using System.Collections;

/// <summary>
/// some action interface
/// </summary>
public interface IAction
{
    /// <summary>
    /// random method
    /// </summary>
    void OnStart();

    /// <summary>
    /// random method
    /// </summary>
    void OnFinish();

    /// <summary>
    /// random method
    /// </summary>
    void OnUpdate();

    /// <summary>
    /// random method
    /// </summary>
    void GUI();

    /// <summary>
    /// random method
    /// </summary>
    /// <returns>if finished</returns>
    bool Finished();
}
