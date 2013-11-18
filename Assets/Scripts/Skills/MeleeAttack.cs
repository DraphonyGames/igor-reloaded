using UnityEngine;
using System.Collections;

/// <summary>
/// Igor's fist melee attack skill.
/// </summary>
public class MeleeAttack : BaseSkill
{
    /// <summary>
    /// sound on hitting an enemy
    /// </summary>
    public AudioClip punchSound;

    /// <summary>
    /// effect played on hit
    /// </summary>
    public GameObject hitEffect;

    /// <summary>
    /// fist damage
    /// </summary>
    public float initialDamage;

    /// <summary>
    /// cooldown in seconds
    /// </summary>
    protected float cooldown = 0.8f;

    /// <summary>
    /// How fast is the attack?
    /// </summary>
    public float MeleeAttackSpeed
    {
        get
        {
            return GetMeleeAttackSpeed(_level);
        }
    }

    /// <summary>
    /// How much damage does the skill cast?
    /// </summary>
    /// <param name="level">The level of the skill</param>
    /// <returns>The damage of the attack.</returns>
    protected float GetDamage(int level)
    {
        return initialDamage * Mathf.Exp(0.15f * (level - 1));
    }

    /// <summary>
    /// How fast is the attack at given level?
    /// </summary>
    /// <param name="level">The level of the skill</param>
    /// <returns>The speed of the attack (>= 1)</returns>
    protected float GetMeleeAttackSpeed(int level)
    {
        return Mathf.Exp(0.15f * (level - 1));
    }

    /// <summary>
    /// Save the level to give back correct attack speed to outer world.
    /// </summary>
    protected int _level = 1;

    /// <summary>Called when the skill goes to another level.</summary>
    /// <param name="by">game object that levels the skill up</param>
    /// <param name="igorInstance">igor (class instance) IFF igor levels the skill</param>
    /// <param name="level">new level of the skill (starts at 1)</param>
    /// <returns>whether the level up was successful</returns>
    public override bool OnLevelChange(GameObject by, Igor igorInstance, int level)
    {
        _level = level;
        return true;
    }

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>The skill name.</returns>
    public override string GetName()
    {
        return "Fist";
    }

    /// <summary>Called when the skill is being used</summary>
    /// <param name="by">game object that uses the skill</param>
    /// <param name="igorInstance">igor (class instance) IFF igor uses the skill</param>
    /// <param name="level">level of the skill (starts at 1)</param>
    /// <returns>whether the usage was successful</returns>
    public override bool OnUse(GameObject by, Igor igorInstance, int level)
    {
        CommonEntity ce = by.GetComponent<CommonEntity>();
        MeleeAttackHelper[] mahs = ce.GetComponentsInChildren<MeleeAttackHelper>();

        MeleeAttackHelper mah = null;
        foreach (MeleeAttackHelper possible_mah in mahs)
        {
            if (!possible_mah.name.Equals("SecondaryFist"))
            {
                mah = possible_mah;
            }
        }

        if (mah == null)
        {
            return false;
        }

        if (ce.IsLocked())
        {
            return false;
        }
        ce.Lock();

        StartCoroutine(AttackCooldownAnimation(ce, mah, level));
        return true;
    }

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">The level of the attack</param>
    /// <returns>The cooldown time.</returns>
    public override float GetCooldown(int level)
    {
        return cooldown / GetMeleeAttackSpeed(level);
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">The level of the skill.</param>
    /// <returns>The needed energy to use the skill.</returns>
    public override float GetNeededEnergy(int level)
    {
        return 0;
    }

    /// <summary>
    /// play attack and reset cooldown
    /// </summary>
    /// <param name="ce">The common entity instance.</param>
    /// <param name="mah">The melee attack helper component.</param>
    /// <param name="level">The level of this skill</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected virtual IEnumerator AttackCooldownAnimation(CommonEntity ce, MeleeAttackHelper mah, int level)
    {
        if (ce is Igor)
        {
            (ce as Igor).RotateToCameraHard();
        }

        ce.StartAnimation(GetName());

        if (ce is Igor)
        {
            yield return new WaitForSeconds(cooldown / 2 / GetMeleeAttackSpeed(level));
        }
        else
        {
            yield return new WaitForSeconds(ce.GetAnimationLength(GetName()) * 0.2f);
        }

        mah.ce = ce;
        mah.attackSkill = this;
        mah.faction = ce.currentFaction;
        mah.attackDamage = GetDamage(level);
        mah.collider.enabled = true;

        if (ce is Igor)
        {
            yield return new WaitForSeconds(cooldown / 2 / GetMeleeAttackSpeed(level));
        }
        else
        {
            yield return new WaitForSeconds(ce.GetAnimationLength(GetName()) * 0.8f);
        }
        mah.alreadyHit = false;
        mah.collider.enabled = false;

        ce.Unlock();
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
        mah.PlaySound(punchSound);
        enemy.TakeDamage(damage);
        Instantiate(hitEffect, fistTransform.position + (collider.bounds.center - fistTransform.position) / 2, Quaternion.identity);
    }
}
