using UnityEngine;
using System.Collections;

/// <summary>
/// Attack which selects the next enemy inside our range and attacks it.
/// </summary>
public class LightningAttack : BaseSkill
{
    /// <summary>
    /// The lightning we are going to instantiate.
    /// </summary>
    public GameObject lightningPrefab;

    /// <summary>
    /// The lightning we are going to instantiate when not hitting.
    /// </summary>
    public GameObject lightningMissPrefab;

    /// <summary>
    /// The sound which should be played when firing.
    /// </summary>
    public AudioClip sound;

    /// <summary>
    /// Duration of the animation.
    /// </summary>
    private float animationDuration = 2.2f;

    /// <summary>
    /// Initial damage without skill points
    /// </summary>
    [SerializeField]
    private float initialDamage = 25f;

    /// <summary>
    /// The initial distance of our attack.
    /// </summary>
    [SerializeField]
    private float initialRange = 20f;

    /// <summary>
    /// Return the damage at given level.
    /// </summary>
    /// <param name="level">The skill level.</param>
    /// <returns>The damage.</returns>
    public float GetDamage(int level)
    {
        return initialDamage * Mathf.Exp(0.2f * (level - 1));
    }

    /// <summary>
    /// Return the range at given level.
    /// </summary>
    /// <param name="level">The skill level.</param>
    /// <returns>The range.</returns>
    public float GetRange(int level)
    {
        return initialRange * Mathf.Exp(0.2f * (level - 1));
    }

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>a name</returns>
    public override string GetName()
    {
        return "Lightning";
    }
    /// <summary>
    /// returns mandatory skills
    /// </summary>
    /// <returns>the skills</returns>
    public override string[] GetNeededSkills()
    {
        return new string[] { "Fist" };
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>The items needed to learn this skill.</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "Coil" };
    }

    /// <summary>
    /// called when used
    /// </summary>
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
    /// <returns>some energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 30f;
    }

    /// <summary>
    /// Play the animation and spawn projectiles.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">the level</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected virtual IEnumerator AttackCooldownAnimation(CommonEntity ce, int level)
    {
        Igor igor;
        if (igor = ce.GetComponent<Igor>())
        {
            igor.RotateToCameraHard();
        }

        // start animation and wait
        ce.StartAnimation(GetName());
        yield return new WaitForSeconds(1f);

        Transform startTransform = null;
        try
        {
            startTransform = ce.GetComponentInChildren<LightningSourceMarker>().transform;
        }
        catch (System.Exception)
        {
            startTransform = ce.transform;
        }

        // Find next enemy -- if it does not exist we'll cast a ray just forward
        CommonEntity enemy = ce.FindNearestEnemy(GetRange(level), 90f);
        GameObject lightningObject;
        if (enemy == null)
        {
            lightningMissPrefab.GetComponent<Lightning>().toPosition = startTransform.position + ce.transform.forward * 20;
            lightningObject = (GameObject)Instantiate(lightningMissPrefab);
        }
        else
        {
            lightningObject = (GameObject)Instantiate(lightningPrefab);
        }

        // Create the lightning and give it the needed positions
        Lightning lightning = lightningObject.GetComponent<Lightning>();
        lightning.fromTransform = startTransform;
        if (enemy != null)
        {
            lightning.toPosition = enemy.collider.bounds.center - enemy.transform.position;
            lightning.toTransform = enemy.transform;
            enemy.TakeDamage(GetDamage(level));
        }

        if (sound)
        {
            Game.PlayAudioAtParent(sound, ce.transform);
        }

        yield return new WaitForSeconds(animationDuration - 1f);
        Destroy(lightningObject);
        ce.Unlock();
    }
}
