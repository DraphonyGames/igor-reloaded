using UnityEngine;
using System.Collections;

/// <summary>
/// rocket of the boss enemy
/// </summary>
public class BossRocket : MonoBehaviour
{
    /// <summary>
    /// damage which the colliding object will take 
    /// </summary>
    public int damage;

    /// <summary>
    /// speed of the Rocket
    /// </summary>
    public float speed = 1;

    /// <summary>
    /// effect which will played by a hit
    /// </summary>
    public GameObject hitEffect;

    /// <summary>
    /// current move direction
    /// </summary>
    private Vector3 moveDirection;

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void Start()
    {
        moveDirection = Vector3.forward;
    }

    /// <summary>
    /// Standard Unity
    /// </summary>
    private void Update()
    {
        moveDirection += Game.GetIgor().transform.position - transform.position + new Vector3(0, 4, 0);
        moveDirection.Normalize();
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    /// <summary>
    /// instanciate explosion by collision
    /// </summary>
    /// <param name="collision">object with which the rocket collide</param>
    private void OnCollisionEnter(Collision collision)
    {
        CommonEntity entity = collision.collider.gameObject.GetComponent<CommonEntity>();

        if (collision.collider.tag.Contains("Player"))
        {
            entity.TakeDamage(20);

        }

        // Ignore boss
        if (collision.gameObject.tag != "Boss")
        {
            if (hitEffect)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }

            Destroy(this.gameObject);
        }
    }
}
