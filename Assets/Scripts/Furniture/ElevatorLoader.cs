using UnityEngine;
using System.Collections;

/// <summary>
/// elevator loader
/// </summary>
public class ElevatorLoader : MonoBehaviour
{
    /// <summary>
    /// run before start to destruct itself if there is already an elevator loader on the same position
    /// </summary>
    public void Awake()
    {
        GameObject[] elevatorLoaders = GameObject.FindGameObjectsWithTag("ElevatorLoader");
        foreach (GameObject elevatorLoader in elevatorLoaders)
        {
            if (elevatorLoader.transform.position.Equals(transform.position) && elevatorLoader != gameObject)
            {
                Destroy(gameObject);
                return;
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
