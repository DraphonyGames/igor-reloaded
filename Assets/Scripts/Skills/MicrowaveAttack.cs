using UnityEngine;
using System.Collections;

/// <summary>
/// Microwave Attack which spreads like a sphere and hits everything.
/// </summary>
public class MicrowaveAttack : BaseSkill
{
    /// <summary>
    /// The microwave sphere.
    /// </summary>
    public GameObject microwaveSpherePrefab;

    /// <summary>
    /// The sound which should be played when firing.
    /// </summary>
    public AudioClip sound;

    /// <summary>
    /// Initial damage without skill points
    /// </summary>
    public float initialDamage = 5;

    /// <summary>
    /// The initial range of the microwave attack.
    /// </summary>
    public float initialRange = 30f;

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>skill name</returns>
    public override string GetName()
    {
        return "Microwave";
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>list of items</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "Microwave" };
    }

    /// <summary>Called when the skill is being used</summary>
    /// <param name="by">game object that uses the skill</param>
    /// <param name="igorInstance">igor (class instance) IFF igor uses the skill</param>
    /// <param name="level">level of the skill (starts at 1)</param>
    /// <returns>whether the usage was successful</returns>
    public override bool OnUse(GameObject by, Igor igorInstance, int level)
    {
        CommonEntity ce = by.GetComponent<CommonEntity>();

        if (ce.IsLocked())
        {
            return false;
        }
        ce.Lock();

        StartCoroutine(AttackCooldownAnimation(ce, level));
        return true;
    }

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    public override float GetCooldown(int level)
    {
        return 10f;
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 35f;
    }

    /// <summary>
    /// Play the animation and spawn the sphere.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">the level</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected virtual IEnumerator AttackCooldownAnimation(CommonEntity ce, int level)
    {
        // start animation, play sound and wait
        ce.StartAnimation(GetName());
        Game.PlayAudioAtParent(sound, ce.transform);
        yield return new WaitForSeconds(2f);

        // Create projectile
        GameObject sphere = (GameObject)Instantiate(microwaveSpherePrefab, ce.collider.bounds.center, Quaternion.identity);
        MicrowaveAttackSphere mas = sphere.GetComponent<MicrowaveAttackSphere>();
        mas.parentTransform = ce.transform;
        mas.parentTranslation = ce.collider.bounds.center - ce.transform.position;
        mas.damage = initialDamage * Mathf.Exp(level * 0.2f);
        mas.maxScale = initialRange * Mathf.Exp(level * 0.2f);

        yield return new WaitForSeconds(1.5f);
        ce.Unlock();
    }
}