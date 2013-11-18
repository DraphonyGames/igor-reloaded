using UnityEngine;
using System.Collections;

/// <summary>
/// a base skill that contains standard implementations for passive-related skills
/// </summary>
public abstract class PassiveBaseSkill : BaseSkill
{
    /// <summary>
    /// Return whether a skill is active (like attacks) or passive (walk speed, jump height, ...)
    /// </summary>
    /// <returns>Whether this skill is passive.</returns>
    public override bool IsPassive()
    {
        return true;
    }

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the cooldown</returns>
    public override float GetCooldown(int level)
    {
        return 0f;
    }

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    public override float GetNeededEnergy(int level)
    {
        return 0f;
    }
}
