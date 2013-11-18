using UnityEngine;
using System.Collections;

/// <summary>
/// general interface for all skills
/// </summary>
public interface ISkill
{
    /// <summary>
    /// Return the skill name.
    /// </summary>
    /// <returns>the name</returns>
    string GetName();

    /// <summary>
    /// Return the corresponding icon.
    /// </summary>
    /// <returns>the texture</returns>
    Texture2D GetIconTexture();

    /// <summary>
    /// called when used
    /// </summary>
    /// <param name="by">game object that uses the skill</param>
    /// <param name="igorInstance">igor (class instance) IFF igor uses the skill</param>
    /// <param name="level">level of the skill (starts at 1)</param>
    /// <returns>whether the usage was successful</returns>
    bool OnUse(GameObject by, Igor igorInstance, int level);

    /// <summary>
    /// similar to OnUse but only called when the player has not enough mana for the skill
    /// when returning true, the skill will be set on cooldown BUT no mana will be drained
    /// this can be used to specify f.e. fallback skills (electro fist may just use fist when low on mana)
    /// </summary>
    /// <param name="by">by who</param>
    /// <param name="igorInstance">the igor</param>
    /// <param name="level">for level</param>
    /// <returns>whether successful</returns>
    bool OnUseWithoutMana(GameObject by, Igor igorInstance, int level);

    /// <summary>Called when the skill goes to another level.</summary>
    /// <param name="by">game object that levels the skill up</param>
    /// <param name="igorInstance">igor (class instance) IFF igor levels the skill</param>
    /// <param name="level">new level of the skill (starts at 1)</param>
    /// <returns>whether the level up was successful</returns>
    bool OnLevelChange(GameObject by, Igor igorInstance, int level);

    /// <summary>
    /// Return whether a skill is active (like attacks) or passive (walk speed, jump height, ...)
    /// </summary>
    /// <returns>Whether this skill is passive.</returns>
    bool IsPassive();

    /// <summary>
    /// returns the skill cooldown in seconds for a specified level
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the cooldown</returns>
    float GetCooldown(int level);

    /// <summary>
    /// returns the needed energy for a specified level
    /// the skill will not be executed if the energy is lower
    /// </summary>
    /// <param name="level">for level</param>
    /// <returns>the energy</returns>
    float GetNeededEnergy(int level);

    /// <summary>
    /// returns a list of skill names that are needed for this very skill to be learned
    /// </summary>
    /// <returns>list of skills</returns>
    string[] GetNeededSkills();
    /// <summary>
    /// special items that are needed to learn a certain skill
    /// </summary>
    /// <returns>list of items</returns>
    string[] GetNeededItems();
}
