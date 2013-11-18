using UnityEngine;
using System.Collections;

/// <summary>
/// script for prefab to prevent having Igor in some scenes
/// </summary>
public class DestructIgor : MonoBehaviour
{
    /// <summary>
    /// destroys an igor if there is one
    /// </summary>
    protected void Start()
    {
        GameObject igor = Game.GetIgor();

        if (igor)
        {
            Destroy(igor.transform.parent.gameObject);
        }
    }
}
