using UnityEngine;
using System.Collections;

/// <summary>
/// big brother's here
/// </summary>
public class FlyingEyeEnemy : EnemyAI
{
    /// <summary>
    /// base melee attack damage
    /// </summary>
    public float meleeDamage = 30.0f;

    /// <summary>
    /// explosion prefab
    /// </summary>
    public GameObject explosionPrefab = null;

    /// <summary>
    /// internal use
    /// </summary>
    private Animator animator;

    /// <summary>
    /// initial start method
    /// </summary>
    public new void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// to set the right animation
    /// </summary>
    private Vector3 lastPosition = Vector3.zero;

    /// <summary>
    /// Update method
    /// </summary>
    public new void Update()
    {
        base.Update();

        animator.SetFloat("walkingSpeed", (transform.position - lastPosition).magnitude / Time.deltaTime);
        lastPosition = transform.position;
    }

    /// <summary>
    /// called when the enemy is in melee range and executes an attack
    /// </summary>
    /// <param name="player">a player</param>
    public override void OnMeleeAttack(GameObject player)
    {
        CommonEntity entity = player.GetComponent<CommonEntity>();
        entity.TakeDamage(meleeDamage);

        // start animation
        animator.SetInteger("currentAttack", (int)Random.Range(0, 3) + 1);
        StartMeleeAttackCooldown(2);
        StartCoroutine(ResetCurrentAttackValue());
    }

    /// <summary>
    /// this is used to reset the attack animation, because of Unity's sub-par animation system
    /// </summary>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    public IEnumerator ResetCurrentAttackValue()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetInteger("currentAttack", 0);
    }

    /// <summary>
    /// takes the damage of
    /// </summary>
    /// <param name="hp">amount of hp taken</param>
    public override void TakeDamage(float hp)
    {
        base.TakeDamage(hp);
    }

    /// <summary>
    /// die method
    /// </summary>
    public override void Die()
    {
        base.Die();
        animator.SetBool("isDead", true);
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
        }

        // this was a pretty rewarding fight!
        ItemDropper.DynamicDrop(transform, 10f);
    }
}