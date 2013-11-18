using UnityEngine;
using System.Collections;

/// <summary>
/// some random transformer class used in the boss fight, but can be hit
/// </summary>
public class Transformer : CommonEntity
{
    /// <summary>
    /// reference to Animator
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Unity Callback
    /// </summary>
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    protected override void Update()
    {
        base.Update();
        if (currentState == EntityState.dead)
        {
            Die();
            healthPoints -= Time.deltaTime * 100;
        }
        animator.SetFloat("Health", healthPoints / maxHealthPoints);
    }

    /// <summary>
    /// When Igor dies we play the die animation and disable the jetpack.
    /// </summary>
    public override void Die()
    {
        base.Die();
        GetComponent<Lightning>().enabled = false;
        GetComponent<CapsuleCollider>().center = new Vector3(GetComponent<CapsuleCollider>().center.x, -2.65f, GetComponent<CapsuleCollider>().center.z);
    }
}
