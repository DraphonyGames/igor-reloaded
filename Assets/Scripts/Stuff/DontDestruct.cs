using UnityEngine;
using System.Collections;

/// <summary>
/// script that can be added to an object to prevent it from being destroyed when a level loads
/// </summary>
public class DontDestruct : MonoBehaviour
{
    /// <summary>
    /// keeps game object from destruction when changing scene
    /// </summary>
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
