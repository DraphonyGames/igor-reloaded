using UnityEngine;
using System.Collections;

/// <summary>
/// Igor's roundhouse attack
/// </summary>
public class RoundhouseAttack : BaseSkill
{
    /// <summary>
    /// The sound which should be played when firing.
    /// </summary>
    public AudioClip sound;

    /// <summary>
    /// Duration of the animation.
    /// </summary>
    private float animationDuration = 0.7f;

    /// <summary>
    /// Initial damage without skill points
    /// </summary>
    public float initialDamage = 25f;

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>a name</returns>
    public override string GetName()
    {
        return "Roundhouse";
    }

    /// <summary>
    /// returns an array with mandatory skills
    /// </summary>
    /// <returns>mandatory skills</returns>
    public override string[] GetNeededSkills()
    {
        return new string[] { "Fist" };
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>needed items</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "Fan" };
    }

    /// <summary>
    /// called when a skill is used
    /// </summary>
    /// <param name="by">game object that uses the skill</param>
    /// <param name="igorInstance">igor (class instance) IFF igor uses the skill</param>
    /// <param name="level">level of the skill (starts at 1)</param>
    /// <returns>whether the usage was successful</returns>
    public override bool OnUse(GameObject by, Igor igorInstance, int level)
    {
        if (by != igorInstance.gameObject)
        {
            return false;
        }

        Igor igor = igorInstance;

        if (igor.IsLocked())
        {
            return false;
        }
        igor.Lock();

        StartCoroutine(AttackCooldownAnimation(igor, level));
        return true;
    }

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the cooldown</returns>
    public override float GetCooldown(int level)
    {
        return animationDuration;
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 10f;
    }

    /// <summary>
    /// How much damage does the skill cast?
    /// </summary>
    /// <param name="level">The level of the skill</param>
    /// <returns>The damage of the attack.</returns>
    private float GetDamage(int level)
    {
        return initialDamage * Mathf.Exp(0.15f * (level - 1));
    }

    /// <summary>
    /// Play the animation and spawn projectiles.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">the level</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected virtual IEnumerator AttackCooldownAnimation(CommonEntity ce, int level)
    {
        Igor igor = (Igor)ce;
        MeleeAttackHelper[] mahs = igor.GetComponentsInChildren<MeleeAttackHelper>();

        foreach (MeleeAttackHelper mah in mahs)
        {
            mah.ce = ce;
            mah.roundhouseSkill = this;
            mah.faction = ce.currentFaction;
            mah.attackDamage = GetDamage(level);
            mah.collider.enabled = true;
            mah.inRoundhouse = true;
            mah.GetComponentInChildren<ParticleSystem>().Play();
        }

        igor.animator.SetInteger("AnimationIndex", 23);

        yield return new WaitForSeconds(animationDuration);

        foreach (MeleeAttackHelper mah in mahs)
        {
            mah.collider.enabled = false;
            mah.inRoundhouse = false;
        }

        igor.animator.SetInteger("AnimationIndex", -1);
        igor.Unlock();
    }

    /// <summary>
    /// Called by the melee attack helper when an enemy is hit.
    /// </summary>
    /// <param name="entity">The entity which casted the attack</param>
    /// <param name="enemy">The enemy which is hit</param>
    /// <param name="collider">The collider of the enemy</param>
    /// <param name="fistTransform">The transform of the melee attack helper component game object.</param>
    /// <param name="damage">The damage which should be put upon the enemy.</param>
    /// <param name="mah">the melee attack helper that calls this function</param>
    public virtual void EnemyHit(CommonEntity entity, CommonEntity enemy, Collider collider, Transform fistTransform, float damage, MeleeAttackHelper mah)
    {
        mah.PlaySound(sound);
        enemy.TakeDamage(damage);
    }
}