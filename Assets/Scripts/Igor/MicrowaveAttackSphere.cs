using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The sphere which is cast by a microwave attack.
/// </summary>
public class MicrowaveAttackSphere : MonoBehaviour
{
    /// <summary>
    /// Damage the bullet does to the player/enemy
    /// </summary>
    public float damage;

    /// <summary>
    /// Toggles damage to the player
    /// </summary>
    public bool damagesPlayer;

    /// <summary>
    /// Toggles damage to the player
    /// </summary>
    public bool damagesEnemies;

    /// <summary>
    /// When something is hit, one object of HitPrefab is created at
    /// the position of the bullet.
    /// </summary>
    public GameObject hitPrefab;

    /// <summary>
    /// Maximum size (in scale units) of the sphere.
    /// </summary>
    public float maxScale = 50f;

    /// <summary>
    /// The parent transform which should be our center all the time.
    /// </summary>
    public Transform parentTransform;

    /// <summary>
    /// The translation from out parent transform.
    /// </summary>
    public Vector3 parentTranslation;

    /// <summary>
    /// The outer sphere (which can be seen from outside)
    /// </summary>
    private GameObject outerSphere;

    /// <summary>
    /// The inner sphere (which can be seen from inside)
    /// </summary>
    private GameObject innerSphere;

    /// <summary>
    /// Save a list of all objects we have already hit.
    /// </summary>
    private List<GameObject> hitObjects = new List<GameObject>();

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
        outerSphere = transform.GetChild(0).gameObject;
        innerSphere = transform.GetChild(1).gameObject;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // Make the sphere bigger
        transform.localScale *= 1f + (5 * Time.deltaTime);
        if (transform.localScale.x > maxScale)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = parentTransform.position + parentTranslation;

        // Make it less visible
        float shininess = transform.localScale.x / maxScale / 3f;
        outerSphere.renderer.material.SetFloat("_Shininess", shininess);
        innerSphere.renderer.material.SetFloat("_Shininess", shininess);

        // Check for collisions with objects
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x);
        foreach (Collider collider in hitColliders)
        {
            if (
                ((damagesEnemies && collider.tag.Contains("Enemy")) || (damagesPlayer && collider.tag.Contains("Player")))
                && !hitObjects.Contains(collider.gameObject))
            {
                hitObjects.Add(collider.gameObject);

                CommonEntity entity = (CommonEntity)collider.gameObject.GetComponent<CommonEntity>();
                entity.TakeDamage(damage * (1.1f - (transform.localScale.x / maxScale)));

                // Create some kind of effect, if set
                if (hitPrefab)
                {
                    Vector3 position = collider.rigidbody != null ? collider.rigidbody.position : collider.bounds.center;
                    Instantiate(hitPrefab, position, Quaternion.identity);
                }
            }
        }
    }
}