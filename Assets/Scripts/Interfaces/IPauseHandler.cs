using UnityEngine;
using System.Collections;

/// <summary>
/// Interface for classes for them to be notified on pause changes
/// </summary>
public interface IPauseHandler
{
    /// <summary>
    /// Called by the Game object when the pause status changes
    /// </summary>
    void PauseStateChanged();
}
