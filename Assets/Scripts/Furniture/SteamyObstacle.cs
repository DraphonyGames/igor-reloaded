using UnityEngine;
using System.Collections;

/// <summary>
/// THIS IS HOT - Steam which kills when touched
/// </summary>
public class SteamyObstacle : MonoBehaviour
{
    /// <summary>
    /// Unity callback
    /// </summary>
    /// <param name="other">a collider</param>
    public void OnTriggerStay(Collider other)
    {
        if (other.tag.Contains("Unit"))
        {
            CommonEntity victim = other.GetComponent<CommonEntity>();
            victim.TakeDamage(5);
        }
    }
}
