using UnityEngine;
using System.Collections;

/// <summary>
/// hack attack which can overtake enemies to fight on your side (only works in syrian nuclear reactors)
/// </summary>
public class HackAttack : BaseSkill
{
    /// <summary>
    /// effect that will be attached to the hacked object
    /// </summary>
    public GameObject hackingEffectPrefab = null;

    /// <summary>
    /// The sound which should be played when firing.
    /// </summary>
    public AudioClip sound;

    /// <summary>
    /// Time in seconds until the hacking effect is reverted.
    /// </summary>
    public float duration = 10f;

    /// <summary>
    /// unity start
    /// </summary>
    public void Start()
    {
        sound = (AudioClip)Resources.Load("wololo3");
    }

    /// <summary>
    /// Get the seconds until the hacking effect should be reverted.
    /// </summary>
    /// <param name="level">The level of the skill.</param>
    /// <returns>Time in seconds until the hacking effect is reverted.</returns>
    public float GetDuration(int level)
    {
        return duration * Mathf.Exp(0.2f * (level - 1));
    }

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>a name</returns>
    public override string GetName()
    {
        return "Hacking";
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>list of items</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "USB Stick" };
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
    /// <returns>the cooldown</returns>
    public override float GetCooldown(int level)
    {
        return 20f - (float)level * 2f;
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 50f;
    }

    /// <summary>
    /// Play the animation and spawn the sphere.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">for level</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected virtual IEnumerator AttackCooldownAnimation(CommonEntity ce, int level)
    {
        // start animation, play sound and wait
        ce.StartAnimation(GetName());
        Game.PlayAudioAtParent(sound, ce.transform);
        yield return new WaitForSeconds(2f);

        CommonEntity enemy = GetEnemyForHacking(ce);

        if (enemy) // might have DIED a SLOW death already, how UNFORTUNATE
        {
            enemy.ChangeFaction();
            if (hackingEffectPrefab)
            {
                GameObject obj = (GameObject)Instantiate(hackingEffectPrefab, enemy.gameObject.transform.position + Vector3.up * 2f, Quaternion.identity);
                obj.transform.parent = enemy.gameObject.transform;
                StartCoroutine(RevertHackEffect(GetDuration(level), enemy, obj));
            }
        }

        yield return new WaitForSeconds(1.5f);
        ce.Unlock();
    }

    /// <summary>
    /// Revert the hack effect after some time.
    /// </summary>
    /// <param name="waitDuration">The time to wait.</param>
    /// <param name="entity">The hacked entity.</param>
    /// <param name="effect">The effect object.</param>
    /// <returns>An enumerator.</returns>
    private IEnumerator RevertHackEffect(float waitDuration, CommonEntity entity, GameObject effect)
    {
        yield return new WaitForSeconds(waitDuration);
        entity.ChangeFaction();
        Destroy(effect);
    }

    /// <summary>
    /// looks for enemy to take over
    /// prefer closer enemies BUT favor enemies directly in your LOS
    /// </summary>
    /// <param name="caster">who uses the skill</param>
    /// <returns>common entity</returns>
    protected CommonEntity GetEnemyForHacking(CommonEntity caster)
    {
        CommonEntity enemy = null;
        float goodness = 0f;
        float maxRange = 20f;
        float maxAngle = 90f;

        Collider[] colliders = Physics.OverlapSphere(caster.gameObject.transform.position, maxRange);

        foreach (Collider collider in colliders)
        {
            CommonEntity entity = collider.gameObject.GetComponent<CommonEntity>();
            if (!entity || entity.gameObject == caster.gameObject || entity.IsFriend(caster) || !entity.IsHackable())
            {
                continue;
            }
            float rangeFactor = maxRange - (caster.gameObject.transform.position - entity.gameObject.transform.position).magnitude;
            Vector3 direction = entity.gameObject.transform.position - caster.gameObject.transform.position;
            float angleFactor = Vector3.Angle(direction, caster.transform.forward);
            if (angleFactor > 90f)
            {
                continue;
            }
            angleFactor = angleFactor * (maxRange / maxAngle);
            float entityGoodness = rangeFactor + angleFactor;

            if (enemy == null || entityGoodness < goodness)
            {
                enemy = entity;
                goodness = entityGoodness;
            }
        }
        return enemy;
    }
}