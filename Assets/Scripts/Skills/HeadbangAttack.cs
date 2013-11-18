using UnityEngine;
using System.Collections;

/// <summary>
/// Igor's head bang
/// </summary>
public class HeadbangAttack : BaseSkill
{
    /// <summary>
    /// sound on hitting an enemy
    /// </summary>
    public AudioClip hitSound;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    public GameObject hitParticleEffect;

    /// <summary>
    /// the name of the function/variable represents the desired behavior and effects quite efficiently. Please read it and interpret it as you like. You will most likely get the correct interpretation.
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>damage for level</returns>
    public float GetDamageForLevel(int level)
    {
        return 10f + 5f * (float)level;
    }

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>The skill name.</returns>
    public override string GetName()
    {
        return "Headbang";
    }

    /// <summary>
    /// names of requirements
    /// </summary>
    /// <returns>The skill name</returns>
    public override string[] GetNeededSkills()
    {
        return new string[] { "Fist" };
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>The skill name</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "Spring" };
    }

    /// <summary>Called when the skill is being used</summary>
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

        if (igorInstance.IsLocked())
        {
            return false;
        }
        igorInstance.Lock();

        StartCoroutine(AttackCooldownAnimation(igorInstance, level));
        return true;
    }

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">The level of the attack</param>
    /// <returns>The cooldown time.</returns>
    public override float GetCooldown(int level)
    {
        return 5f - 0.75f * (float)level;
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
    /// play attack and unlock
    /// </summary>
    /// <param name="igor">The igor instance.</param>
    /// <param name="level">The level of this skill</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    private IEnumerator AttackCooldownAnimation(Igor igor, int level)
    {
        igor.RotateToCameraHard();

        igor.StartAnimation(GetName());

        yield return new WaitForSeconds(0.3f);

        HeadMarker marker = igor.gameObject.GetComponentInChildren<HeadMarker>();

        Collider[] colliders = Physics.OverlapSphere(marker.transform.position + marker.transform.forward - marker.transform.right, 2f);

        foreach (Collider collider in colliders)
        {
            CommonEntity entity = collider.gameObject.GetComponent<CommonEntity>();
            if (!entity)
            {
                continue;
            }

            if (igor.IsFriend(entity))
            {
                continue;
            }

            entity.TakeDamage(GetDamageForLevel(level));

            if (hitParticleEffect)
            {
                Instantiate(hitParticleEffect, 0.5f * (igor.transform.position + collider.transform.position) + Vector3.up * igor.collider.bounds.extents.y, Quaternion.identity);
            }

            if (hitSound)
            {
                Game.PlayAudioAtParent(hitSound, marker.transform);
            }
        }

        yield return new WaitForSeconds(0.35f);
        igor.Unlock();
    }
}
