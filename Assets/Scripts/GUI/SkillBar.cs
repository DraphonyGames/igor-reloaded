using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// on-screen skill bar which is always displayed
/// the player can see the currently selected skills as well as items that are assigned to hotkeys
/// </summary>
public class SkillBar : MonoBehaviour
{
    #region StaticInterface

    /// <summary>
    /// allow NO less (but also no more)
    /// </summary>
    private readonly int maxHotkeyCount = 10;

    /// <summary>
    /// the only instance, it's a singleton! Magic.
    /// </summary>
    private static SkillBar skillBarInstance = null;

    /// <summary>
    /// retrieves the SkillBar that is currently in game
    /// </summary>
    /// <returns>yeah, well, the instance</returns>
    public static SkillBar GetInstance()
    {
        if (skillBarInstance == null)
        {
            skillBarInstance = (SkillBar)((GameObject)Instantiate(Resources.Load("SkillBarPrefab"))).GetComponent<SkillBar>();
            UnityEngine.Object.DontDestroyOnLoad(skillBarInstance);
        }
        return skillBarInstance;
    }

    #endregion

    /// <summary>
    /// shown when skills/items are on cooldown
    /// </summary>
    public Texture2D cooldownBarTexture = null;

    /// <summary>
    /// left mouse button symbol
    /// </summary>
    public Texture2D mouse1Texture = null;

    /// <summary>
    /// right mouse button symbol
    /// </summary>
    public Texture2D mouse2Texture = null;

    /// <summary>
    /// one item in the skill bar
    /// </summary>
    public class SkillBarItem
    {
        /// <summary>
        /// the type of the item
        /// </summary>
        public enum ItemType
        {
            /// <summary>
            /// It's an invalid item.
            /// </summary>
            INVALID,
            /// <summary>
            /// It's an item type.
            /// </summary>
            ITEM,
            /// <summary>
            /// It's an skill type.
            /// </summary>
            SKILL
        }
        /// <summary>
        /// the texture of the icon
        /// </summary>
        private Texture2D iconTexture;

        /// <summary>
        /// the name of the icon, will be used to acquire said REAL item
        /// </summary>
        public string name;

        /// <summary>
        /// the type of the item
        /// </summary>
        public ItemType type;

        /// <summary>
        /// current cooldown for that item
        /// </summary>
        private float cooldown = 0f;

        /// <summary>
        /// to calculate the width of the cooldown bar
        /// </summary>
        private float lastMaxCooldown = 0f;

        /// <summary>
        /// constructs a new skill bar item, it's a constructor
        /// </summary>
        /// <param name="_type">the type</param>
        /// <param name="_name">the name</param>
        /// <param name="_iconTexture">an image</param>
        public SkillBarItem(ItemType _type, string _name, Texture2D _iconTexture)
        {
            type = _type;
            name = _name;
            iconTexture = _iconTexture;
        }

        /// <summary>
        /// starts a cooldown
        /// </summary>
        /// <param name="seconds">how long</param>
        public void StartCooldown(float seconds)
        {
            lastMaxCooldown = cooldown = seconds;
        }

        /// <summary>
        /// decreases the cooldown
        /// </summary>
        public void DecCooldown()
        {
            cooldown -= 1f * Time.deltaTime;
            if (cooldown < 0f)
            {
                cooldown = 0f;
            }
        }
        /// <summary>
        /// if cooldown is active
        /// </summary>
        /// <returns>true or false</returns>
        public bool HasCooldown()
        {
            return cooldown > 0f;
        }

        /// <summary>
        /// returns the part of the cooldown still remaining (between 0 and 1)
        /// </summary>
        /// <returns>normalized between 0 and 1</returns>
        public float GetRelativeCooldown()
        {
            if (lastMaxCooldown == 0f)
            {
                return 0f;
            }
            return cooldown / lastMaxCooldown;
        }

        /// <summary>
        /// returns a name
        /// </summary>
        /// <returns>a name</returns>
        public string GetName()
        {
            if (name != "")
            {
                return name;
            }
            return "EMPTY";
        }
        /// <summary>
        /// the texture
        /// </summary>
        /// <returns>some texture</returns>
        public Texture2D GetTexture()
        {
            return iconTexture;
        }
    }

    /// <summary>
    /// contains the currently displayed (thus selected) items and skills
    /// </summary>
    public List<SkillBarItem> items = new List<SkillBarItem>();

    /// <summary>
    /// constructs a skill bar
    /// </summary>
    public SkillBar()
    {
        for (int i = 0; i < maxHotkeyCount; ++i)
        {
            items.Add(null);
        }
    }

    /// <summary>
    /// Unity Update
    /// </summary>
    private void Update()
    {
        if (!Game.GetIgor() || !Game.GetIgorComponent().IsAlive() || Game.isMenuOpen || Game.IsPaused || Game.IsCutscene)
        {
            return;
        }

        int hotkeyOffset = 2; // 2 mouse buttons!
        int currentHotkey = -1;

        if (Input.GetButtonDown("Fire1"))
        {
            currentHotkey = 0;
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            currentHotkey = 1;
        }

        if (currentHotkey == -1)
        {
            for (int i = hotkeyOffset; i < maxHotkeyCount; ++i)
            {
                if (!Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i - hotkeyOffset)))
                {
                    continue;
                }
                currentHotkey = i;
                break;
            }
        }
        if (currentHotkey == -1)
        {
            return;
        }

        OnHotkeyPressed(currentHotkey);
    }

    /// <summary>
    /// when an item should be executed
    /// </summary>
    /// <param name="currentHotkey">which one</param>
    public void OnHotkeyPressed(int currentHotkey)
    {
        SkillBarItem item = items[currentHotkey];
        if (item == null)
        {
            return;
        }

        if (item.HasCooldown())
        {
            // MessageBoard.AddMessage(item.GetName() + " is still on cooldown!");
            return;
        }

        switch (item.type)
        {
            case SkillBarItem.ItemType.ITEM:
                Inventory inventory = Game.GetIgor().GetComponent<Inventory>();

                if (inventory == null)
                {
                    Debug.Log("No inventory in Igor!");
                    return;
                }

                if (!inventory.HasItem(item.GetName()))
                {
                    MessageBoard.AddMessage("You don't have any " + item.GetName());
                    return;
                }
                inventory.UseItem(item.GetName());
                break;

            case SkillBarItem.ItemType.SKILL:
                Skills.SkillData skillData = Skills.GetSkill(item.GetName());
                ISkill skill = skillData.skill;
                int level = skillData.level;

                GameObject igorGameObject = Game.GetIgor();
                Igor igor = igorGameObject.GetComponent<Igor>();
                bool executed = false;
                bool lowManaFallbackUsed = false;

                if (skill.GetNeededEnergy(level) > igor.GetMana())
                {
                    lowManaFallbackUsed = skill.OnUseWithoutMana(igorGameObject, igor, level);
                    if (lowManaFallbackUsed)
                    {
                        executed = true;
                    }
                    else
                    {
                        MessageBoard.AddMessage("Not enough mana!");
                        return;
                    }
                }

                if (!executed)
                {
                    executed = skill.OnUse(igorGameObject, igor, level);
                }

                if (executed)
                {
                    item.StartCooldown(skill.GetCooldown(level));
                    if (!lowManaFallbackUsed)
                    {
                        igor.DoMana(-skill.GetNeededEnergy(level));
                    }
                }
                break;
        }
    }

    /// <summary>
    /// put item/skill on slot
    /// </summary>
    /// <param name="index">the index</param>
    /// <param name="item">some item</param>
    /// <param name="type">item type</param>
    /// <param name="texture">the texture</param>
    public void AssignSlot(int index, string item, SkillBarItem.ItemType type, Texture2D texture = null)
    {
        if (texture == null)
        {
            switch (type)
            {
                case SkillBarItem.ItemType.SKILL:
                    texture = Skills.GetSkill(item).skill.GetIconTexture();
                    break;
                case SkillBarItem.ItemType.ITEM:
                    texture = Inventory.GetTextureForItem(item);
                    break;
            }
        }
        items[index] = new SkillBarItem(type, item, texture);
    }

    /// <summary>
    /// Unity OnGUI
    /// </summary>
    private void OnGUI()
    {
        if ((!Game.InGame && !Game.isMenuOpen) || !Game.GetIgor())
        {
            return;
        }

        float itemSize = 64f;
        // for very low resolutions, half the item size. Otherwise keep the big icons
        if (Screen.width < 2f * itemSize * items.Count)
        {
            itemSize /= 2f;
        }

        float itemMargin = itemSize / 4f;
        float totalWidth = (itemMargin + itemSize) * items.Count;

        MessageBoard.DrawMessageBackground((int)((Screen.width - totalWidth) * 0.5f), (int)(Screen.height - (itemMargin + itemSize)), (int)totalWidth, (int)itemSize);

        DragNDrop.MetaData dropMetaData = DragNDrop.GetDrop();

        int count = -1;
        foreach (SkillBarItem item in items)
        {
            ++count;

            float x = count * (itemMargin + itemSize) - totalWidth * 0.5f + Screen.width * 0.5f;
            float y = Screen.height - (itemMargin + itemSize);
            Rect rect = new Rect(x, y, itemSize, itemSize);

            // assign new?
            if ((dropMetaData != null) && rect.Contains(new Vector2(dropMetaData.x, dropMetaData.y)))
            {
                SkillBarItem.ItemType type = SkillBarItem.ItemType.INVALID;
                switch (dropMetaData.source)
                {
                    case DragNDrop.MetaData.Source.SKILL_TREE:
                        type = SkillBarItem.ItemType.SKILL;
                        break;
                    case DragNDrop.MetaData.Source.INVENTORY:
                        type = SkillBarItem.ItemType.ITEM;
                        break;
                }
                if (type != SkillBarItem.ItemType.INVALID)
                {
                    AssignSlot(count, dropMetaData.info, type, dropMetaData.drawingTexture);
                }
            }

            // draw!
            if (item != null)
            {
                Texture2D iconTex = item.GetTexture();
                GUI.Box(rect, "");
                if (iconTex)
                {
                    GUI.DrawTexture(rect, iconTex);
                }
            }
            else
            {
                GUI.Box(rect, "");
            }

            // draw mouse symbol or number icon
            Rect buttonInfoRect = new Rect(rect.x, rect.y + rect.height * 0.5f, rect.width * 0.5f, rect.height * 0.5f);
            if (count < 2)
            {
                Texture2D tex = count == 1 ? mouse1Texture : mouse2Texture;
                GUI.DrawTexture(buttonInfoRect, tex);
            }
            else
            {
                GUI.Label(buttonInfoRect, "" + (count - 1));
            }

            // draw cooldown over everything else!
            if (item != null && item.HasCooldown())
            {
                item.DecCooldown();

                Rect barRect = new Rect(rect.x + 5f, rect.y + 5f, item.GetRelativeCooldown() * (rect.width - 10f), rect.height * 0.25f);
                GUI.DrawTexture(barRect, cooldownBarTexture);
            }

            if (item != null && item.type == SkillBarItem.ItemType.ITEM)
            {
                // check whether the item is not anymore in the inventory
                Inventory inventory = Game.GetIgor().GetComponent<Inventory>();
                if (inventory != null && !inventory.HasItem(item.name))
                {
                    SkillTree.DrawCross(rect.x, rect.y, rect.width, rect.height, new Rect(0, 0, 0, 0), Color.red, 5f);
                }
            }
        }
    }
}
