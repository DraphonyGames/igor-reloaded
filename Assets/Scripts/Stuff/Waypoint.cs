using UnityEngine;
using System.Collections;

/// <summary>
/// Waypoint script
/// </summary>
public class Waypoint : MonoBehaviour
{
    /// <summary>
    /// just destruct the renderer
    /// </summary>
    protected void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
    }
}
