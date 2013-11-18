using UnityEngine;
using System.Collections;

/// <summary>
/// the mighty prism, super laser attack
/// </summary>
public class PrismAttack : LaserAttack
{
    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>a name</returns>
    public override string GetName()
    {
        return "Prism Laser";
    }

    /// <summary>
    /// parent skills
    /// </summary>
    /// <returns>some skills</returns>
    public override string[] GetNeededSkills()
    {
        return new string[] { "Laser" };
    }

    /// <summary>
    /// some items
    /// </summary>
    /// <returns>needed items</returns>
    public override string[] GetNeededItems()
    {
        return new string[] { "Prism" };
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for which level</param>
    /// <returns>the energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 25f;
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
        if (level > attackLevels.Length)
        {
            return false;
        }
        return base.OnUse(by, igorInstance, level);
    }

    /// <summary>
    /// Define how the laser attack should work.
    /// </summary>
    private enum LaserAttack
    {
        Horizontal, Vertical, Cross, Area
    }

    /// <summary>
    /// The attack types per level.
    /// </summary>
    private LaserAttack[] attackLevels = { LaserAttack.Horizontal, LaserAttack.Cross, LaserAttack.Area, LaserAttack.Area };

    /// <summary>
    /// The number of projectiles per attack level.
    /// </summary>
    private int[] attackProjectiles = { 3, 3, 3, 4 };

    /// <summary>
    /// Spawn the projectiles.
    /// </summary>
    /// <param name="ce">common entity</param>
    /// <param name="level">for what level</param>
    protected override void SpawnProjectiles(CommonEntity ce, int level)
    {
        LaserAttack attackType = attackLevels[level - 1];
        int projectiles = attackProjectiles[level - 1];
        float projectileAngleStep = 5f;

        float maxAnglePerSide = (projectiles - 1) / 2f * projectileAngleStep;

        float damagePerProjectile = initialLaserDamage * Mathf.Exp(level * 0.2f);
        float factor = projectiles > 0 ? projectiles : 1;
        damagePerProjectile = 1 + damagePerProjectile / factor;

        for (float angleV = -maxAnglePerSide; angleV <= maxAnglePerSide; angleV += projectileAngleStep)
        {
            for (float angleH = -maxAnglePerSide; angleH <= maxAnglePerSide; angleH += projectileAngleStep)
            {
                if (
                    (attackType == LaserAttack.Vertical && angleH != 0) ||
                    (attackType == LaserAttack.Horizontal && angleV != 0) ||
                    (attackType == LaserAttack.Cross && angleH * angleV != 0))
                {
                    continue;
                }
                SpawnProjectile(ce, damagePerProjectile, angleH, angleV);
            }
        }
    }
}
