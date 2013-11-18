using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// system for handling skills, learning and leveling
/// </summary>
public static class Skills
{
    /// <summary>
    /// all skills that are available to Igor
    /// </summary>
    private static readonly string[] SkillNames = new string[]
    {
        "MeleeAttackPrefab",
        "ElectroFistAttackPrefab",
        "LaserAttackPrefab",
        "PrismAttackPrefab",
        "LightningAttackPrefab",
        "MicrowaveAttackPrefab",
        "Igor_Max_Health_Points",
        "Igor_Health_Regeneration",
        "Igor_Time_Until_Regeneration",
        "Igor_Speed",
        "Igor_Jump_Force",
        "Igor_Jump_Force_Regeneration",
        "Igor_Max_Mana",
        "Igor_Mana_Regeneration",
        "HackAttackPrefab",
        "HeadbangAttackPrefab",
        "Igor_Roundhouse_Attack",
    };

    /// <summary>
    /// keeps track of skill stuff
    /// </summary>
    public class SkillData
    {
        /// <summary>
        /// skill of the skill data
        /// </summary>
        public ISkill skill;
        /// <summary>
        /// level of the skill
        /// </summary>
        public int level = 0;
        /// <summary>
        /// constructs something
        /// </summary>
        /// <param name="_skill">to skill</param>
        /// <param name="_level">to level</param>
        public SkillData(ISkill _skill, int _level = 0)
        {
            skill = _skill;
            level = _level;
        }
        /// <summary>
        /// returns whether two skills are related (parent-child-relationship)
        /// </summary>
        /// <param name="otherSkillData">other skill</param>
        /// <returns>whether related</returns>
        public bool IsRelated(SkillData otherSkillData)
        {
            if (IsChildOf(otherSkillData) || IsParentOf(otherSkillData))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// in a skill tree sense
        /// </summary>
        /// <param name="otherSkillData">other skill</param>
        /// <returns>whether child</returns>
        public bool IsChildOf(SkillData otherSkillData)
        {
            return Array.IndexOf(skill.GetNeededSkills(), otherSkillData.skill.GetName()) != -1;
        }
        /// <summary>
        /// see above
        /// </summary>
        /// <param name="otherSkillData">other skill</param>
        /// <returns>whether parent</returns>
        public bool IsParentOf(SkillData otherSkillData)
        {
            return otherSkillData.IsChildOf(this);
        }
    }

    /// <summary>
    /// keeps instances of the skill prefabs
    /// </summary>
    private static List<SkillData> skills = new List<SkillData>();

    /// <summary>
    /// clears the skill list
    /// this also resets all of the levels etc.
    /// </summary>
    public static void SkillsClear()
    {
        skills.Clear();
    }

    /// <summary>
    /// instantiates all of the skill prefabs
    /// </summary>
    /// <returns>whether the skills have been instantiated - if false, they were kept</returns>
    public static bool InstantiateSkills()
    {
        // already instantiated?
        if (skills.Count > 0 && skills[0] != null)
        {
            return false;
        }
        // clear to remove possible nullified prefab instances (after loading new scene f.e.)
        skills.Clear();

        foreach (string name in SkillNames)
        {
            GameObject obj = (GameObject)Resources.Load(name);
            if (obj)
            {
                obj = (GameObject)UnityEngine.Object.Instantiate(obj);
                UnityEngine.Object.DontDestroyOnLoad(obj);
            }

            if (!obj)
            {
                Debug.LogError("Could not load skill " + name);
                continue;
            }
            ISkill skill = null;

            foreach (MonoBehaviour mono in obj.GetComponents(typeof(MonoBehaviour)))
            {
                skill = mono as ISkill;
                if (skill != null)
                {
                    break;
                }
            }
            if (skill == null)
            {
                Debug.LogError("Skill " + name + " does not implement ISkill!");
                continue;
            }

            skills.Add(new SkillData(skill));
        }
        return true;
    }

    /// <summary>
    /// returns a skill (if existing) with the specified display name (which is not the name of the prefab)
    /// </summary>
    /// <param name="displayName">display name of the skill</param>
    /// <returns>see above</returns>
    public static SkillData GetSkill(string displayName)
    {
        foreach (SkillData skill in skills)
        {
            if (skill.skill.GetName() == displayName)
            {
                return skill;
            }
        }
        return null;
    }

    /// <summary>
    /// returns all possible skills that can be learned by Igor
    /// </summary>
    /// <returns>yeah, see above</returns>
    public static List<SkillData> GetSkills()
    {
        List<SkillData> copy = new List<SkillData>();
        copy.AddRange(skills);
        return copy;
    }

    /// <summary>
    /// whether Igor currently knows a certain skill
    /// </summary>
    /// <param name="name">name of skill</param>
    /// <returns>I just said that</returns>
    public static bool KnowsSkill(string name)
    {
        foreach (SkillData skill in skills)
        {
            if (skill.skill.GetName() != name)
            {
                continue;
            }
            return skill.level > 0;
        }
        return false;
    }

    /// <summary>
    /// increases the level of a skill
    /// </summary>
    /// <param name="name">which skill</param>
    /// <returns>whether successful</returns>
    public static bool LevelUpSkill(string name)
    {
        foreach (SkillData skill in skills)
        {
            if (skill.skill.GetName() != name)
            {
                continue;
            }
            if (skill.level >= 4)
            {
                return false;
            }
            ++skill.level;
            skill.skill.OnLevelChange(Game.GetIgor(), Game.GetIgorComponent(), skill.level);
            return true;
        }
        return false;
    }

    /// <summary>
    /// reset skill data
    /// </summary>
    public static void ResetSkillsData()
    {
        List<Skills.SkillData> skills = Skills.GetSkills();
        foreach (Skills.SkillData skill in skills)
        {
            skill.skill.OnLevelChange(Game.GetIgor(), Game.GetIgorComponent(), skill.level);
        }
    }

    /// <summary>
    /// sets the level of a skill
    /// </summary>
    /// <param name="name">which skill</param>
    /// <param name="level">to which level</param>
    public static void SetSkillLevel(string name, int level)
    {
        foreach (SkillData skill in skills)
        {
            if (skill.skill.GetName() != name)
            {
                continue;
            }
            skill.level = level;
            skill.skill.OnLevelChange(Game.GetIgor(), Game.GetIgorComponent(), skill.level);
            return;
        }
    }
}
