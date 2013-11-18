using UnityEngine;
using System.Collections;

/// <summary>
/// transporting conveyor
/// </summary>
public class Conveyor : MonoBehaviour
{
    /// <summary>
    /// speed of movement
    /// </summary>
    public float speed = 1;

    /// <summary>
    /// Unity Callback
    /// </summary>
    /// <param name="collider">a collider</param>
    private void OnTriggerStay(Collider collider)
    {

        if (collider.GetComponent<Rigidbody>() != null)
        {
            collider.gameObject.transform.position -= transform.up * Time.deltaTime * speed;
        }
    }
}
