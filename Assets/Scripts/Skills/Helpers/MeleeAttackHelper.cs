using UnityEngine;
using System.Collections;

/// <summary>
/// Manages damage for melee attack
/// Add the IgorPrimaryAttack prefab as child of the Igor GameObject to the bone which should damage the enemy.
/// </summary>
public class MeleeAttackHelper : MonoBehaviour
{
    /// <summary>
    /// Damage of the attack
    /// </summary>
    public float attackDamage;

    /// <summary>
    /// indicates whether an enemy is already hit (prevent multiple hits in one melee attack)
    /// </summary>
    internal bool alreadyHit = false; // Idea: change it to int, to get limited multihits

    /// <summary>
    /// indicates who's enemies to look for
    /// </summary>
    public CommonEntity.Faction faction = CommonEntity.Faction.PLAYER;

    /// <summary>
    /// The corresponding attack skill.
    /// </summary>
    public MeleeAttack attackSkill;

    /// <summary>
    /// corresponding roundhouse attack skill
    /// </summary>
    public RoundhouseAttack roundhouseSkill;

    /// <summary>
    /// indicates whether igor is in a roundhouse attack or not
    /// </summary>
    internal bool inRoundhouse = false;

    /// <summary>
    /// The common entity to which this helper belongs.
    /// </summary>
    public CommonEntity ce;

    /// <summary>
    /// deactivate collider on startup.
    /// </summary>
    private void Start()
    {
        collider.enabled = false;
        collider.isTrigger = true;
    }

    /// <summary>
    /// handles a collision and give the enemy damage
    /// </summary>
    /// <param name="other">some collider</param>
    private void OnTriggerStay(Collider other)
    {
        if (!alreadyHit && !inRoundhouse)
        {
            CommonEntity entity = other.gameObject.GetComponent<CommonEntity>();
            if (entity == null || entity.IsFriend(faction))
            {
                return;
            }

            alreadyHit = true;

            attackSkill.EnemyHit(ce, entity, other, transform, attackDamage, this);
        }
    }

    /// <summary>
    /// plays a sound
    /// </summary>
    /// <param name="sound">AudioClip that should be played</param>
    public void PlaySound(AudioClip sound)
    {
        if (!audio)
        {
            gameObject.AddComponent<AudioSource>();
        }
        audio.PlayOneShot(sound);
    }

    /// <summary>
    /// executed on collision
    /// </summary>
    /// <param name="other">collider information</param>
    protected void OnTriggerEnter(Collider other)
    {
        if (inRoundhouse)
        {
            CommonEntity entity = other.gameObject.GetComponent<CommonEntity>();
            if (entity == null || entity.IsFriend(faction))
            {
                return;
            }
            alreadyHit = true;

            roundhouseSkill.EnemyHit(ce, entity, other, transform, attackDamage, this);
        }
    }
}
