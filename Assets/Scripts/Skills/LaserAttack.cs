using UnityEngine;
using System.Collections;

/// <summary>
/// the laser attack
/// </summary>
public class LaserAttack : BaseSkill
{
    /// <summary>
    /// The projectile we are going to instantiate.
    /// </summary>
    public GameObject projectilePrefab;

    /// <summary>
    /// The sound which should be played when firing.
    /// </summary>
    private AudioClip sound;

    /// <summary>
    /// Duration of the animation.
    /// </summary>
    private float animationDuration = 2.7f;

    /// <summary>
    /// Initial laser damage without skill points
    /// </summary>
    public float initialLaserDamage = 25f;

    /// <summary>
    /// unity start
    /// </summary>
    private void Start()
    {
        sound = (AudioClip)Resources.Load("Laser_Cannon-Mike_Koenig");
    }

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>a name</returns>
    public override string GetName()
    {
        return "Laser";
    }
    /// <summary>
    /// mandatory skills
    /// </summary>
    /// <returns>list of skills</returns>
    public override string[] GetNeededSkills()
    {
        return new string[] { "Fist" };
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
    /// <returns>the energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 5f;
    }

    /// <summary>
    /// Play the animation and spawn projectiles.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">for level</param>
    /// <returns>An enumerator needed for yielding, just throw it away</returns>
    protected virtual IEnumerator AttackCooldownAnimation(CommonEntity ce, int level)
    {
        Igor igor;
        if (igor = ce.GetComponent<Igor>())
        {
            igor.RotateToCameraHard();
        }

        // start animation, play sound and wait
        ce.StartAnimation(GetName());
        Game.PlayAudioAtParent(sound, ce.transform);
        yield return new WaitForSeconds(animationDuration * 1 / 4);

        // Create projectile
        SpawnProjectiles(ce, level);

        yield return new WaitForSeconds(animationDuration * 1 / 4);
        ce.Unlock();
    }

    /// <summary>
    /// Spawn the projectiles.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">for level</param>
    protected virtual void SpawnProjectiles(CommonEntity ce, int level)
    {
        SpawnProjectile(ce, initialLaserDamage * Mathf.Exp(level * 0.2f));
    }

    /// <summary>
    /// Spawn a projectile with a certain /damage/. Rotate optionally by a vertical and horizontal angle.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="damage">The damage made by this projectile.</param>
    /// <param name="horizontalAngle">The vertical angle.</param>
    /// <param name="verticalAngle">The horizontal angle.</param>
    protected virtual void SpawnProjectile(CommonEntity ce, float damage, float horizontalAngle = 0f, float verticalAngle = 0f)
    {
        GameObject projectileGO = (GameObject)Instantiate(projectilePrefab, ce.collider.bounds.center + ce.transform.forward * (2f + Mathf.Max(ce.collider.bounds.extents.x, ce.collider.bounds.extents.z)), Quaternion.identity);

        // calculate rotation
        Quaternion rotation = ce.transform.rotation;
        rotation = Quaternion.AngleAxis(horizontalAngle, ce.transform.up) * rotation;
        rotation = Quaternion.AngleAxis(verticalAngle, ce.transform.right) * rotation;
        projectileGO.transform.rotation = rotation;

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.damage = damage;
        projectile.faction = ce.GetFaction();
    }
}