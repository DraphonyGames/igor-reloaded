using UnityEngine;
using System.Collections;

/// <summary>
/// Basic projectile class. 
/// 
/// Bullet moves into Y direction of the object and can hit both player and enemies, depending on the faction set.
/// The bullet will only hit things that are hostile to the bullet's faction OR that are targeted "Hittable" (for stationary targets,  f.e.)
/// Has to be applied onto a rigid body and is removed after hitting something.
/// </summary>
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Damage the bullet does to the player/enemy
    /// </summary>
    public float damage;

    /// <summary>
    /// Amount of damage dealt per interval. For more information see IHitable.
    /// </summary>
    public float damagePerIntervall;

    /// <summary>
    /// Time until the damage stops. For more information see IHitable.
    /// </summary>
    public float damageIntervallLength;

    /// <summary>
    /// Interval in which damage is dealt
    /// </summary>
    public float damageTime;

    /// <summary>
    /// Speed of the bullet. Bullet moves into Y direction of the object.
    /// </summary>
    public float speed;

    /// <summary>
    /// Time until the bullet gets destroyed.
    /// 
    /// Defaults to 10 seconds if not set.
    /// </summary>
    public float maxTimeAlive;

    /// <summary>
    /// Timestamp of the creation of the bullet, used for deleting the bullet after a certain time
    /// </summary>
    private float timeOfCreation;

    /// <summary>
    /// the bullet belongs to a faction and will hit only other factions
    /// set to "NONE" if you want to hit anything
    /// </summary>
    public CommonEntity.Faction faction;

    /// <summary>
    /// When something is hit, one object of HitPrefab is created at
    /// the position of the bullet.
    /// </summary>
    public GameObject hitPrefab;

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Start()
    {
        timeOfCreation = Time.time;

        if (maxTimeAlive == 0)
        {
            maxTimeAlive = 10;
        }

        // set the speed for the rigid body
        Vector3 localSpeed = transform.forward.normalized * speed;
        rigidbody.AddForce(localSpeed, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    private void Update()
    {
        if (timeOfCreation + maxTimeAlive <= Time.time)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called by Unity.
    /// Damages every CommonEntity of other faction OR every "Hitable"-tagged object that implements IHitable
    /// Bullet is removed after hit.
    /// </summary>
    /// <param name="collider">Set by Unity</param>
    private void OnTriggerEnter(Collider collider)
    {
        // do NOT collide with other triggers..
        if (collider.isTrigger)
        {
            return;
        }

        CommonEntity entity = collider.gameObject.GetComponent<CommonEntity>();

        if (entity && !entity.IsFriend(faction))
        {
            entity.TakeDamage(damage);
            entity.TakeDamageOverTime(damagePerIntervall, damageIntervallLength, damageTime);
        }
        else if (collider.tag.Contains("Hitable"))
        {
            foreach (MonoBehaviour mono in collider.gameObject.GetComponents<MonoBehaviour>())
            {
                IHitable hitable = mono as IHitable;
                if (hitable == null)
                {
                    continue;
                }
                hitable.TakeDamage(damage);
                hitable.TakeDamageOverTime(damagePerIntervall, damageIntervallLength, damageTime);
                if (collider.gameObject == null)
                {
                    break;
                }
            }
        }

        // Create some kind of effect, if set
        if (hitPrefab)
        {
            Instantiate(hitPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
