using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

/// <summary>
/// The class for all passive skills.
/// </summary>
public class PassiveIgorSkill : PassiveBaseSkill
{
    /// <summary>
    /// The name of this skill.
    /// </summary>
    private string skillName;

    /// <summary>
    /// The name of the corresponding property inside Igor. Derived from skillName.
    /// </summary>
    private string propertyName;

    /// <summary>
    /// The name of the corresponding initial property inside Igor. Derived from skillName.
    /// </summary>
    private string initialPropertyName;

    /// <summary>
    /// Scaling factor for all passive skills.
    /// </summary>
    private static float factor = 0.2f;

    /// <summary>
    /// return the skill name.
    /// </summary>
    /// <returns>the name</returns>
    public override string GetName()
    {
        return skillName;
    }

    /// <summary>
    /// Called when being instantiated.
    /// </summary>
    private void Awake()
    {
        skillName = name.Substring(5, name.Length - 12).Replace("_", " ");
        propertyName = skillName.Replace(" ", string.Empty);
        initialPropertyName = "initial" + propertyName;
        propertyName = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
    }

    /// <summary>Called when the skill goes to another level.</summary>
    /// <param name="by">game object that levels the skill up</param>
    /// <param name="igorInstance">igor (class instance) IFF igor levels the skill</param>
    /// <param name="level">new level of the skill (starts at 1)</param>
    /// <returns>whether the level up was successful</returns>
    public override bool OnLevelChange(GameObject by, Igor igorInstance, int level)
    {
        if (by != igorInstance.gameObject)
        {
            return false;
        }

        Type igorType = typeof(Igor);
        FieldInfo propertyField = igorType.GetField(propertyName);
        FieldInfo initialPropertyField = igorType.GetField(initialPropertyName);
        propertyField.SetValue(igorInstance, ((float)initialPropertyField.GetValue(igorInstance)) * Mathf.Exp((level - 1) * factor));

        return true;
    }

    /// <summary>
    /// other skills that are needed to learn a certain skill
    /// </summary>
    /// <returns>An array of the needed skill to get this skill.</returns>
    public override string[] GetNeededSkills()
    {
        switch (skillName)
        {
            case "Health Regeneration":
                return new string[] { "Max Health Points" };

            case "Jump Force":
                return new string[] { "Jump Force Regeneration" };

            case "Jump Force Regeneration":
                return new string[] { "Speed" };

            case "Mana Regeneration":
                return new string[] { "Max Mana" };

            case "Max Health Points":
                return new string[] { };

            case "Max Mana":
                return new string[] { };

            case "Speed":
                return new string[] { };

            case "Time Until Regeneration":
                return new string[] { "Health Regeneration" };
        }

        return new string[] { };
    }
}