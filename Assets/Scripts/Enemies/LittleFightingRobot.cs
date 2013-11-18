using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// small but deadly
/// </summary>
public class LittleFightingRobot : EnemyAI
{
    /// <summary>
    /// explosion prefab
    /// </summary>
    private GameObject explosionPrefab;

    /// <summary>
    /// instance of the smoke prefab when robot died
    /// </summary>
    private GameObject smoke;

    /// <summary>
    /// smoke prefab
    /// </summary>
    private GameObject smokePrefab;

    /// <summary>
    /// the robot's shot
    /// </summary>
    public GameObject projectilePrefab;

    /// <summary>
    /// audio when played when shooting
    /// </summary>
    public AudioClip projectileShotAudioClip;

    /// <summary>
    /// used for setting the walk speed for the animation controller
    /// </summary>
    private Vector3 lastPosition = Vector3.zero;

    /// <summary>
    /// the character animator
    /// </summary>
    private Animator animator = null;

    /// <summary>
    /// Called by Unity
    /// </summary>
    protected new void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        type = EnemyTpye.Range;
        smokePrefab = (GameObject)Resources.Load("Detailed Smoke");
        explosionPrefab = (GameObject)Resources.Load("Explosion");
    }


    /// <summary>
    /// Called by Unity
    /// </summary>
    public new void Update()
    {
        base.Update();

        float walkingSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        animator.SetFloat("walkingSpeed", walkingSpeed);
    }

    /// <summary>
    /// called when ready for a ranged attack
    /// </summary>
    public override void OnRangeAttack()
    {
        StartCoroutine("TakeRangeAttack");
        StartRangeAttackCooldown(.5f);
    }

    /// <summary>
    /// execute the ranged attack
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    public IEnumerator TakeRangeAttack()
    {
        animator.SetBool("inAttack", true);
        yield return new WaitForSeconds(0.01f);
        Vector3 projectilePosition = transform.collider.bounds.center + 4f * transform.forward * transform.localScale.z - 2f * transform.right;
        projectilePosition.x -= 0.8f;

        Game.PlayAudioAtParent(projectileShotAudioClip, transform);
        GameObject projectile = (GameObject)Instantiate(projectilePrefab, projectilePosition, Quaternion.identity);

        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.transform.position;
            if (currentTarget.collider != null)
            {
                targetPosition = currentTarget.collider.bounds.center;
            }
            projectile.transform.LookAt(targetPosition);
        }

        yield return new WaitForSeconds(0.2f);
        animator.SetBool("inAttack", false);
    }


    /// <summary>
    /// called when dying
    /// </summary>
    public override void Die()
    {
        base.Die();
        rigidbody.constraints = RigidbodyConstraints.FreezeAll | RigidbodyConstraints.FreezeRotation;
        smoke = (GameObject)Instantiate(smokePrefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// called eventually when DEAD
    /// </summary>
    internal override void Destruct()
    {
        base.Destruct();

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (gameObject != null)
        {
            Destroy(gameObject);
            Destroy(smoke);
        }

        // this was a pretty rewarding fight!
        ItemDropper.DynamicDrop(transform, 4f);
    }
}
