using UnityEngine;
using System.Collections;

/// <summary>
/// the electro fist skill
/// </summary>
public class ElectroFistAttack : MeleeAttack
{
    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>The skill name.</returns>
    public override string GetName()
    {
        return "Electro Fist";
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>The needed items to learn this skill.</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "Coil" };
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">The level of the skill.</param>
    /// <returns>The needed energy to use the skill.</returns>
    public override float GetNeededEnergy(int level)
    {
        return 10f;
    }

    /// <summary>
    /// similar to OnUse but only called when the player has not enough mana for the skill
    /// when returning true, the skill will be set on cooldown BUT no mana will be drained
    /// this can be used to specify f.e. fallback skills (electro fist may just use fist when low on mana)
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="igorInstance">the igor</param>
    /// <param name="level">for level</param>
    /// <returns>true or false</returns>
    public override bool OnUseWithoutMana(GameObject by, Igor igorInstance, int level)
    {
        return Skills.GetSkill("Fist").skill.OnUse(by, igorInstance, level);
    }

    /// <summary>
    /// play attack and reset cooldown
    /// </summary>
    /// <param name="ce">The common entity instance.</param>
    /// <param name="mah">The melee attack helper component.</param>
    /// <param name="level">The level of this skill</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected override IEnumerator AttackCooldownAnimation(CommonEntity ce, MeleeAttackHelper mah, int level)
    {
        try
        {
            (ce as Igor).RotateToCameraHard();
        }
        catch (System.Exception)
        {

        }

        ce.StartAnimation(GetName());

        yield return new WaitForSeconds(cooldown / 2 / GetMeleeAttackSpeed(level));
        mah.ce = ce;
        mah.attackSkill = this;
        mah.faction = ce.currentFaction;
        mah.attackDamage = GetDamage(level);
        mah.collider.enabled = true;

        yield return new WaitForSeconds(cooldown / 2 / GetMeleeAttackSpeed(level));
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
    /// <param name="mah">the melee attack helper that calls this function</param>s
    public override void EnemyHit(CommonEntity entity, CommonEntity enemy, Collider collider, Transform fistTransform, float damage, MeleeAttackHelper mah)
    {
        mah.PlaySound(punchSound);

        GameObject effect = (GameObject)Instantiate(hitEffect, fistTransform.position + fistTransform.forward * fistTransform.localScale.z + new Vector3(0, 0, 0), Quaternion.AngleAxis(90, fistTransform.right));
        Lightning lightning;
        if (lightning = effect.GetComponent<Lightning>())
        {
            lightning.fromTransform = fistTransform;
            lightning.toTransform = enemy.transform;
            lightning.toPosition = enemy.collider.bounds.center - enemy.transform.position;
        }
        enemy.TakeDamage(damage);
        enemy.TakeDamageOverTime(damage / 20f, 0.5f, 2f);

        Destroy(effect, 0.2f);
    }
}
