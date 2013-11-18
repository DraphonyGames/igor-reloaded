using UnityEngine;
using System.Collections;

/// <summary>
/// the infamous janitor
/// </summary>
public class DiagnosisRobotEnemy : EnemyAI
{
    /// <summary>
    /// base melee attack damage
    /// </summary>
    public float meleeDamage = 10.0f;

    /// <summary>
    /// smoke prefab
    /// </summary>
    private GameObject smokePrefab;

    /// <summary>
    /// explosion prefab
    /// </summary>
    private GameObject explosionPrefab;

    /// <summary>
    /// instance of the smoke prefab when robot died
    /// </summary>
    private GameObject smoke;

    /// <summary>
    /// the peep sound when dead
    /// </summary>
    private AudioClip peepSound;

    /// <summary>
    /// initial start method
    /// </summary>
    public new void Start()
    {
        base.Start();
        type = EnemyTpye.Melee;
        smokePrefab = (GameObject)Resources.Load("Detailed Smoke");
        explosionPrefab = (GameObject)Resources.Load("Explosion");
        peepSound = (AudioClip)Resources.Load("DiagnosisRobotEnemyPeep");
    }

    /// <summary>
    /// Update method
    /// </summary>
    public new void Update()
    {
        base.Update();
        if (currentState == EntityState.dead)
        {
            // keep track of the smoke, because robot might slip a little after death
            smoke.transform.position = transform.position;
        }
    }

    /// <summary>
    /// called when the enemy is in melee range and executes an attack
    /// </summary>
    /// <param name="player">a player</param>
    public override void OnMeleeAttack(GameObject player)
    {
        CommonEntity entity = player.GetComponent<CommonEntity>();
        entity.TakeDamage(meleeDamage);

        StartMeleeAttackCooldown(2);
        ExecuteEscape(3.0f);
    }

    /// <summary>
    /// takes the damage of
    /// </summary>
    /// <param name="hp">amount of hp taken</param>
    public override void TakeDamage(float hp)
    {
        base.TakeDamage(hp);
        if (currentState == EntityState.normal && !InWayPoint())
        {
            ExecuteEscape(3.0f);
        }
    }

    /// <summary>
    /// die method
    /// </summary>
    public override void Die()
    {
        base.Die();

        // make sure that you cannot get stuck in diagnosis robots who slide into you while dying
        if (collider != null)
        {
            Destroy(collider);
        }

        rigidbody.constraints = RigidbodyConstraints.FreezeAll | RigidbodyConstraints.FreezeRotation;

        audio.PlayOneShot(peepSound);
        smoke = (GameObject)Instantiate(smokePrefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// callback by common entity
    /// </summary>
    internal override void Destruct()
    {
        base.Destruct();

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // am I dead? :(
        if (gameObject != null)
        {
            Destroy(gameObject);
            Destroy(smoke);
        }

        // this was a pretty rewarding fight!
        ItemDropper.DynamicDrop(transform, 3f);
    }
}