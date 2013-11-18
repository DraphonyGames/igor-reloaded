using UnityEngine;
using System.Collections;

/// <summary>
/// Stationary arm enemy
/// </summary>
public class ArmEnemy : EnemyAI
{
    /// <summary>
    /// the damage
    /// </summary>
    public float meleeAttackDamage = 20f;

    /// <summary>
    /// reference to the animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// range in which the arm reacts to Igor
    /// </summary>
    public float viewRange = 2;

    /// <summary>
    /// range in which the arm attacks Igor
    /// </summary>
    public float attackRange = 2;

    /// <summary>
    /// flag for the attack which is currently running
    /// </summary>
    public int inAttack = 0;

    /// <summary>
    /// cooldown for attack 1
    /// </summary>
    public float attack1Cooldown = 1;

    /// <summary>
    /// cooldown for attack 2
    /// </summary>
    public float attack2Cooldown = 1;

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
    /// Called by Unity
    /// </summary>
    protected new void Start()
    {
        base.Start();
        type = EnemyTpye.Melee;
        currentState = EntityState.normal;
        animator = GetComponent<Animator>();
        smokePrefab = (GameObject)Resources.Load("Detailed Smoke");
        explosionPrefab = (GameObject)Resources.Load("Explosion");
    }

    /// <summary>
    /// Called by Unity
    /// </summary>
    protected new void Update()
    {
        base.Update();

        CommonEntity target = GetCurrentTarget();
        if (target != null && healthPoints > 0)
        {
            if (Vector3.Distance(target.gameObject.transform.position, transform.position) < viewRange)
            {
                Vector3 lookAt = target.gameObject.transform.position - transform.position;
                lookAt.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookAt);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);

                if (Vector3.Distance(target.gameObject.transform.position, transform.position) < attackRange && inAttack == 0)
                {
                    inAttack = 1;
                    StartCoroutine("Attack1CooldownAnimation");
                }
            }
        }
    }


    /// <summary>
    /// Calculate Animation And Damage
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator Attack1CooldownAnimation()
    {
        EnemyHitUnit enemyHitUnit = (EnemyHitUnit)GetComponentInChildren<EnemyHitUnit>();
        enemyHitUnit.enable = true;
        enemyHitUnit.faction = GetFaction();
        enemyHitUnit.damage = meleeAttackDamage;
        animator.SetInteger("Animation", 1);

        yield return new WaitForSeconds(attack1Cooldown);
        inAttack = 0;

        animator.SetInteger("Animation", 0);
        enemyHitUnit.enable = false;
    }

    /// <summary>
    /// die die die die
    /// </summary>
    public override void Die()
    {
        base.Die();
        animator.SetInteger("Animation", 3);

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        smoke = (GameObject)Instantiate(smokePrefab, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// execute destruction
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
