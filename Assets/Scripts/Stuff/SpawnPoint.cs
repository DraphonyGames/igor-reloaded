using UnityEngine;
using System.Collections;

/// <summary>
/// manages spawn point
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Igor Prefab that should be instantiated
    /// </summary>
    public GameObject igorPrefab;

    /// <summary>
    /// Unity start
    /// </summary>
    protected void Start()
    {
        SkinnedMeshRenderer[] renderer = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer meshRenderer in renderer)
        {
            Destroy(meshRenderer);
        }

        Game.SpawnIgor();
    }

    /// <summary>
    /// spawns an igor given in the prefab
    /// </summary>
    /// <returns>the igor instance</returns>
    public GameObject SpawnIgor()
    {
        GameObject igor = (GameObject)Instantiate(igorPrefab, transform.position, transform.rotation);
        DontDestroyOnLoad(igor);
        return igor;
    }
}