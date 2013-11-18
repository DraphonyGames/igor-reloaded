using UnityEngine;
using System.Collections;

/// <summary>
/// base skill class with standard definitions for certain methods
/// </summary>
public abstract class BaseSkill : MonoBehaviour, ISkill
{
    /// <summary>
    /// Save the corresponding icon.
    /// </summary>
    protected Texture2D iconTexture;

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>a name</returns>
    public abstract string GetName();

    /// <summary>
    /// Return the corresponding icon.
    /// </summary>
    /// <returns>the texture</returns>
    public virtual Texture2D GetIconTexture()
    {
        if (!iconTexture)
        {
            iconTexture = (Texture2D)Resources.Load("SkillIcons/" + GetName());
        }
        return iconTexture;
    }

    /// <summary>
    /// returns a list of skill names that are needed for this very skill to be learned
    /// </summary>
    /// <returns>list of skills</returns>
    public virtual string[] GetNeededSkills()
    {
        return new string[] { };
    }

    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>list of items</returns>
    public virtual string[] GetNeededItems()
    {
        return new string[] { };
    }

    /// <summary>
    /// called when used
    /// </summary>
    /// <summary>Called when the skill is being used</summary>
    /// <param name="by">game object that uses the skill</param>
    /// <param name="igorInstance">igor (class instance) IFF igor uses the skill</param>
    /// <param name="level">level of the skill (starts at 1)</param>
    /// <returns>whether the usage was successful</returns>
    public virtual bool OnUse(GameObject by, Igor igorInstance, int level)
    {
        return false;
    }

    /// <summary>
    /// similar to OnUse but only called when the player has not enough mana for the skill
    /// when returning true, the skill will be set on cooldown BUT no mana will be drained
    /// this can be used to specify f.e. fallback skills (electro fist may just use fist when low on mana)
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="igorInstance">the igor</param>
    /// <param name="level">the level</param>
    /// <returns>true or false</returns>
    public virtual bool OnUseWithoutMana(GameObject by, Igor igorInstance, int level)
    {
        return false;
    }

    /// <summary>Called when the skill goes to another level.</summary>
    /// <param name="by">game object that levels the skill up</param>
    /// <param name="igorInstance">igor (class instance) IFF igor levels the skill</param>
    /// <param name="level">new level of the skill (starts at 1)</param>
    /// <returns>whether the level up was successful</returns>
    public virtual bool OnLevelChange(GameObject by, Igor igorInstance, int level)
    {
        return false;
    }

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the cooldown</returns>
    public abstract float GetCooldown(int level);

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    public abstract float GetNeededEnergy(int level);

    /// <summary>
    /// Return whether a skill is active (like attacks) or passive (walk speed, jump height, ...)
    /// </summary>
    /// <returns>Whether this skill is passive.</returns>
    public virtual bool IsPassive()
    {
        return false;
    }
}
