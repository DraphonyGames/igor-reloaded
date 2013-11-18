using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// the marvelous skill tree
/// </summary>
public class SkillTree : MonoBehaviour
{
    /// <summary>
    /// some margin
    /// </summary>
    public const float MarginLeftRight = 50f;

    /// <summary>
    /// more margin
    /// </summary>
    public const float MarginTopBottom = 100f;

    #region StaticInterface

    /// <summary>
    /// one instance
    /// </summary>
    private static SkillTree skillTreeInstance = null;

    /// <summary>
    /// returns the current skill tree
    /// </summary>
    /// <returns>some instance</returns>
    public static SkillTree GetInstance()
    {
        if (skillTreeInstance == null)
        {
            GameObject prefab = (GameObject)Resources.Load("SkillTreePrefab");
            if (prefab == null)
            {
                Debug.LogError("Could not load skill tree prefab!");
                return null;
            }
            GameObject instance = (GameObject)UnityEngine.Object.Instantiate(prefab);
            if (instance == null)
            {
                Debug.LogError("Could not instantiate skill tree prefab!");
                return null;
            }
            skillTreeInstance = (SkillTree)instance.GetComponent<SkillTree>();
            UnityEngine.Object.DontDestroyOnLoad(skillTreeInstance);
        }
        return skillTreeInstance;
    }

    #endregion

    /// <summary>
    /// symbol that is drawn on the skill icons for every level
    /// </summary>
    public Texture2D levelUpSymbol;

    #region SkillTreeNode

    /// <summary>
    /// one node in the skill tree
    /// </summary>
    private class SkillTreeNode
    {
        /// <summary>
        /// the standard width
        /// </summary>
        private readonly float stdWidth = 64f;
        /// <summary>
        /// some coordinates
        /// </summary>
        public float x, y, wdt, hgt;
        /// <summary>
        /// some data
        /// </summary>
        public Skills.SkillData skillData;
        /// <summary>
        /// the logical parent of this skill, might go both directions
        /// </summary>
        private SkillTreeNode parent;
        /// <summary>
        /// whether the skill can be selected in the skill tree to either upgrade it or learn it
        /// </summary>
        private bool canBeSelected = false;
        /// <summary>
        /// texture + tooltip for the item that is currently lacking
        /// </summary>
        private GUIContent neededItemInformation = null;

        /// <summary>
        /// some tooltip
        /// </summary>
        private string skillTooltip = null;

        /// <summary>
        /// the name to draw
        /// </summary>
        private string drawingName;

        /// <summary>
        /// the icon
        /// </summary>
        private Texture2D drawingTexture;

        /// <summary>
        /// constructs something
        /// </summary>
        /// <param name="_skillData">the data</param>
        /// <param name="_parent">the parent</param>
        public SkillTreeNode(Skills.SkillData _skillData, SkillTreeNode _parent)
        {
            skillData = _skillData;
            parent = _parent;
        }

        /// <summary>
        /// refreshes only soft data (which is NOT the position and layout in the tree)
        /// </summary>
        /// <param name="tree">the parent tree</param>
        public void RefreshState(SkillTree tree)
        {
            canBeSelected = true;
            neededItemInformation = null;
            skillTooltip = null;

            // if not yet learned..
            if (skillData.level == 0)
            {
                // ...check parent skills
                foreach (string pre in skillData.skill.GetNeededSkills())
                {
                    if (Skills.KnowsSkill(pre))
                    {
                        continue;
                    }
                    canBeSelected = false;
                    break;
                }
                // ...and inventory
                Inventory inventory = Game.GetIgor().GetComponent<Inventory>();
                foreach (string pre in skillData.skill.GetNeededItems())
                {
                    neededItemInformation = new GUIContent("", Inventory.GetTextureForItem(pre), "You need a " + pre);
                    if (inventory.HasItem(pre))
                    {
                        continue;
                    }
                    canBeSelected = false;
                    break;
                }

                if (canBeSelected && Game.GetIgorComponent().skillpoints == 0)
                {
                    skillTooltip = "You need skillpoints for this skill!";
                }
            }
            // every skill can be upgraded only a certain amount of times
            if (canBeSelected)
            {
                if (skillData.level >= 4)
                {
                    canBeSelected = false;
                }
                else if (skillTooltip == null)
                {
                    if (skillData.level == 0)
                    {
                        skillTooltip = "Learn " + skillData.skill.GetName();
                    }
                    else
                    {
                        skillTooltip = "Improve " + skillData.skill.GetName();
                    }
                }
            }

            drawingName = skillData.skill.GetName();
            drawingTexture = skillData.skill.GetIconTexture();
            tree.ReportNodePosition(x, y, wdt, hgt);
        }

        /// <summary>
        /// recalculates everything
        /// </summary>
        /// <param name="tree">the parent tree</param>
        public void Refresh(SkillTree tree)
        {
            x = y = 0f;
            wdt = hgt = stdWidth;

            if (Mathf.Min(Screen.height, Screen.width) < 7f * stdWidth)
            {
                wdt = hgt = stdWidth * 0.5f;
            }

            float gridOffsetX = SkillTree.MarginLeftRight + wdt * 0.5f;
            float gridOffsetY = SkillTree.MarginTopBottom;
            float gridMarginX = wdt;
            float gridMarginY = hgt * 0.5f;

            int attempts = 0;
            do
            {
                // std position
                x = gridOffsetX;
                y = gridOffsetY;

                if (parent != null)
                {
                    if (parent.skillData.IsParentOf(skillData))
                    {
                        x = parent.x + parent.wdt + gridMarginX;
                        y = parent.y;
                    }
                    else if (parent.skillData.IsChildOf(skillData))
                    {
                        x = parent.x - gridMarginX;
                    }
                }

                // dynamically try new positions based on previous, failed attempts :<
                if (parent != null)
                {
                    x += gridMarginX * (float)(attempts / 10);
                    y += gridMarginY * (float)(attempts % 10);
                }
                else
                {
                    x += gridMarginX * (attempts / 10);
                    y += gridMarginY * (attempts % 10);
                }
            }
            while (!tree.IsPositionFree(x, y, wdt, hgt) && ++attempts > 0);

            RefreshState(tree);
        }

        /// <summary>
        /// obvious pretty much
        /// </summary>
        /// <param name="tree">the parent tree</param>
        /// <param name="drawArea">Area to draw in - clip otherwise. Set to (0, 0, 0, 0) disable clipping</param>
        public void Draw(SkillTree tree, Rect drawArea)
        {
            // draw connection to parent first!
            Vector2 origin = new Vector2(x - 25f, y + hgt * 0.5f);
            Vector2 target = new Vector2(x, origin.y);

            if (parent != null)
            {
                if (parent.x < x)
                {
                    origin = new Vector2(parent.x + parent.wdt, parent.y + parent.hgt * 0.5f);
                }
                else
                {
                    origin = new Vector2(parent.x, parent.y + parent.hgt * 0.5f);
                    target = new Vector2(x + wdt, y + hgt * 0.5f);
                }
            }
            Drawing.DrawLine(origin, target, drawArea, Color.yellow, 5f);
            Drawing.DrawLine(origin, target, drawArea, Color.black, 2f);

            Rect rect = new Rect(x, y, wdt, hgt);
            if (canBeSelected)
            {
                if (GUI.Button(rect, new GUIContent("", drawingTexture, skillTooltip)))
                {
                    OnSkillSelected(tree);
                }
            }
            else
            {
                GUI.Box(rect, new GUIContent("", drawingTexture, skillTooltip));
            }

            for (int i = 0; i < skillData.level; ++i)
            {
                float size = wdt * 0.25f;
                tree.DrawLevelUpSymbol(x + wdt - size, y + hgt - size * (float)(i + 1), size, size);
            }

            if (skillData.level > 0)
            {
                Color clr = skillData.skill.IsPassive() ? Color.blue : Color.yellow;
                SkillTree.DrawFrame(x, y, wdt, hgt, drawArea, clr, 2f);
            }
            else if (!canBeSelected)
            {
                SkillTree.DrawCross(x, y, wdt, hgt, drawArea, Color.black, 1f);
            }


            if (neededItemInformation != null)
            {
                GUI.Box(new Rect(x - wdt * 0.5f, y + hgt * 0.25f, wdt * 0.5f, hgt * 0.5f), neededItemInformation);
            }

            // check drag&drop
            if (skillData.level > 0 && !skillData.skill.IsPassive() && Input.GetMouseButtonDown(0) && rect.Contains(Event.current.mousePosition))
            {
                DragNDrop.StartDrag(drawingName, DragNDrop.MetaData.Source.SKILL_TREE, drawingTexture);
            }
        }

        /// <summary>
        /// when a skill is selected
        /// </summary>
        /// <param name="tree">still the parent</param>
        private void OnSkillSelected(SkillTree tree)
        {
            Igor igor = Game.GetIgor().GetComponent<Igor>();
            if (igor.skillpoints <= 0)
            {
                return;
            }

            int oldLevel = Skills.GetSkill(skillData.skill.GetName()).level;

            if (!Skills.LevelUpSkill(skillData.skill.GetName()))
            {
                return;
            }

            // remove items from inventory when learning the skill for the very first time
            if (oldLevel == 0)
            {
                Inventory inventory = Game.GetIgor().GetComponent<Inventory>();
                foreach (string itemName in skillData.skill.GetNeededItems())
                {
                    if (!inventory.HasItem(itemName))
                    {
                        MessageBoard.AddMessage("You do not have all the necessary items!");
                        return;
                    }
                    inventory.RemoveItem(itemName);
                }
            }

            MessageBoard.AddMessage("You upgraded skill " + skillData.skill.GetName());
            --igor.skillpoints;

            tree.RefreshAllStates();
        }
    }

    /// <summary>
    /// for just the very best user experience!
    /// </summary>
    public float maxSkillNodePositionX, maxSkillNodePositionY;

    /// <summary>
    /// nodes should, after adjusting to layout, report their position back
    /// </summary>
    /// <param name="x">some coordinate</param>
    /// <param name="y">some coordinate</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    public void ReportNodePosition(float x, float y, float wdt, float hgt)
    {
        if (x + wdt > maxSkillNodePositionX)
        {
            maxSkillNodePositionX = x + wdt;
        }
        if (y + hgt > maxSkillNodePositionY)
        {
            maxSkillNodePositionY = y + hgt;
        }
    }
    #endregion

    /// <summary>
    /// a random list of nodes
    /// </summary>
    private List<SkillTreeNode> nodes = new List<SkillTreeNode>();

    /// <summary>
    /// current state of GUI
    /// </summary>
    private bool isEnabled = false;

    /// <summary>
    /// show the item
    /// </summary>
    public void Show()
    {
        if (Game.isMenuOpen || Game.IsCutscene || Game.GetIgor() == null || !Game.GetIgorComponent().IsAlive())
        {
            return;
        }

        Recalculate();
        isEnabled = true;
        Game.IsPaused = true;
        Game.isMenuOpen = true;
    }

    /// <summary>
    /// hide it again
    /// </summary>
    public void Hide()
    {
        isEnabled = false;
        Game.IsPaused = false;
        Game.isMenuOpen = false;
    }

    /// <summary>
    /// Unity Update
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Skillgui"))
        {
            if (isEnabled)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    /// <summary>
    /// refreshes the states (availability etc.) of all nodes
    /// </summary>
    public void RefreshAllStates()
    {
        foreach (SkillTreeNode node in nodes)
        {
            node.RefreshState(this);
        }
    }

    /// <summary>
    /// recalculates the whole tree
    /// </summary>
    public void Recalculate()
    {
        nodes.Clear();

        List<Skills.SkillData> skills = Skills.GetSkills();
        Queue<SkillTreeNode> recentSkills = new Queue<SkillTreeNode>();

        while (skills.Count > 0)
        {
            Skills.SkillData nextSkill = skills[0];
            SkillTreeNode parentSkill = null;

            bool foundRecent = false;
            while (recentSkills.Count > 0 && !foundRecent)
            {
                SkillTreeNode recent = recentSkills.Peek();

                // look for related skill and priorize it
                foreach (Skills.SkillData relatedSkill in skills)
                {
                    if (!relatedSkill.IsRelated(recent.skillData))
                    {
                        continue;
                    }
                    foundRecent = true;
                    nextSkill = relatedSkill;
                    parentSkill = recent;
                    break;
                }
                // we are DONE here
                if (!foundRecent)
                {
                    recentSkills.Dequeue();
                }
            }

            skills.Remove(nextSkill);

            SkillTreeNode newNode = new SkillTreeNode(nextSkill, parentSkill);
            recentSkills.Enqueue(newNode);

            newNode.Refresh(this);
            nodes.Add(newNode);
        }
    }

    /// <summary>
    /// returns whether a certain position in the graph can be filled by a node
    /// </summary>
    /// <param name="x">the coordinates</param>
    /// <param name="y">the coordinates</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    /// <returns>true or false</returns>
    public bool IsPositionFree(float x, float y, float wdt, float hgt)
    {
        float safetyMargin = wdt * 0.25f;
        foreach (SkillTreeNode node in nodes)
        {
            if (node.x - safetyMargin > x || node.x + node.wdt + safetyMargin < x)
            {
                continue;
            }
            if (node.y - safetyMargin > y || node.y + node.hgt + safetyMargin < y)
            {
                continue;
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// draws a symbol
    /// </summary>
    /// <param name="x">some coordinates</param>
    /// <param name="y">some coordinates</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    private void DrawLevelUpSymbol(float x, float y, float wdt, float hgt)
    {
        GUI.DrawTexture(new Rect(x, y, wdt, hgt), levelUpSymbol);
    }

    /// <summary>
    /// draws a frame
    /// </summary>
    /// <param name="x">some coordinates</param>
    /// <param name="y">some coordinates</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    /// <param name="drawRect">Area to draw in - clip otherwise. Set to (0, 0, 0, 0) disable clipping</param>
    /// <param name="color">some color</param>
    /// <param name="width">line width</param>
    /// <param name="antialease">on or off</param>
    public static void DrawFrame(float x, float y, float wdt, float hgt, Rect drawRect, Color color, float width, bool antialease = true)
    {
        Drawing.DrawLine(new Vector2(x, y), new Vector2(x + hgt, y), drawRect, color, width);
        Drawing.DrawLine(new Vector2(x + wdt, y), new Vector2(x + wdt, y + hgt), drawRect, color, width);
        Drawing.DrawLine(new Vector2(x + wdt, y + wdt), new Vector2(x, y + hgt), drawRect, color, width);
        Drawing.DrawLine(new Vector2(x, y + hgt), new Vector2(x, y), drawRect, color, width);
    }

    /// <summary>
    /// draws a cross
    /// </summary>
    /// <param name="x">some coordinates</param>
    /// <param name="y">some coordinates</param>
    /// <param name="wdt">the width</param>
    /// <param name="hgt">the height</param>
    /// <param name="drawRect">Area to draw in - clip otherwise. Set to (0, 0, 0, 0) disable clipping</param>
    /// <param name="color">some color</param>
    /// <param name="width">line width</param>
    /// <param name="antialease">on or off</param>
    public static void DrawCross(float x, float y, float wdt, float hgt, Rect drawRect, Color color, float width, bool antialease = true)
    {
        Drawing.DrawLine(new Vector2(x, y), new Vector2(x + wdt, y + hgt), drawRect, color, width);
        Drawing.DrawLine(new Vector2(x + wdt, y), new Vector2(x, y + hgt), drawRect, color, width);
    }

    /// <summary>
    /// internal use
    /// </summary>
    private Vector2 scrollViewPosition = Vector2.zero;

    /// <summary>
    /// Unity OnGUI
    /// </summary>
    private void OnGUI()
    {
        if (!isEnabled)
        {
            return;
        }

        float offsetX = MarginLeftRight;
        float offsetY = MarginTopBottom;
        float x = offsetX;
        float y = offsetY;
        float wdt = Screen.width - 2f * offsetX;
        float hgt = Screen.height - 2f * offsetY;

        Rect drawArea = new Rect(x, y, wdt, hgt);

        MessageBoard.DrawMessageBackground(x, y, wdt, hgt);

        scrollViewPosition = GUI.BeginScrollView(drawArea, scrollViewPosition, new Rect(x, y, Mathf.Max(wdt, maxSkillNodePositionX), Mathf.Max(hgt, maxSkillNodePositionY)));
        foreach (SkillTreeNode node in nodes)
        {
            node.Draw(this, new Rect(0, 0, 0, 0)); // empty rec, so no clipping in later DrawLine calls
        }
        GUI.EndScrollView();

        float skillPointSize = 32f;
        Igor igor = Game.GetIgor().GetComponent<Igor>();
        string skillPointsString = (igor.skillpoints == 0) ? "no skillpoints remaining!" : "Skillpoints: ";
        TextAnchor old = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(x, y + hgt - skillPointSize, 150f, skillPointSize), skillPointsString);
        GUI.skin.label.alignment = old;
        for (int i = 0; i < igor.skillpoints; ++i)
        {
            DrawLevelUpSymbol(x + 75f + skillPointSize * (float)i, y + hgt - skillPointSize, skillPointSize, skillPointSize);
        }

        // finally, the tooltip!
        if (GUI.tooltip != "")
        {
            GUI.Box(new Rect(Event.current.mousePosition.x - 150f, Event.current.mousePosition.y - 15f + 32f, 300f, 30f), GUI.tooltip);
        }
    }
}
