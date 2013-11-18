using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Inventory for the player
/// </summary>
public class Inventory : MonoBehaviour
{
    // static interface for caching item textures
    #region StaticInterface

    /// <summary>
    /// caches the icons to be able to access them even without an object in the scene
    /// </summary>
    private static Dictionary<string, Texture2D> itemIconCache = new Dictionary<string, Texture2D>();

    /// <summary>
    /// returns the texture for an item name
    /// </summary>
    /// <param name="itemName">name of item</param>
    /// <returns>the texture</returns>
    public static Texture2D GetTextureForItem(string itemName)
    {
        if (itemIconCache.ContainsKey(itemName))
        {
            return itemIconCache[itemName];
        }
        Texture2D tex = (Texture2D)Resources.Load("ItemIcons/" + itemName);
        if (!tex)
        {
            Debug.LogError("Could not load preview icon for " + itemName);
            return null;
        }
        itemIconCache.Add(itemName, tex);
        return tex;
    }

    #endregion

    /// <summary>
    /// some constructor
    /// </summary>
    public Inventory()
    {
    }

    /// <summary>
    /// Represents an item in the inventory
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// texture of the item, will be cached
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// The instance of the item.
        /// </summary>
        public IInvItem instance;

        /// <summary>
        /// The size of the stack
        /// </summary>
        public int stackS;

        /// <summary>
        /// Initializes a new InventoryItem instance
        /// </summary>
        /// <param name="item">The item type of this stack</param>
        /// <param name="initialStackSize">Size of the stack to start with</param>
        public InventoryItem(IInvItem item, int initialStackSize)
        {
            instance = item;
            stackS = initialStackSize;
        }

        /// <summary>
        /// The name of the item to be shown to the user
        /// </summary>
        public string DisplayName
        {
            get { return instance.GetDisplayName(); }
        }

        /// <summary>
        /// Size of the stack
        /// </summary>
        public int StackSize
        {
            get { return stackS; }
        }

        /// <summary>
        /// Adds the item to the item stack if possible and returns if it worked
        /// </summary>
        /// <param name="item">Item to add to the stack</param>
        /// <returns>True if it could be added to the stack, false if not</returns>
        public bool Stack(IInvItem item)
        {
            // Are we the same item
            if (item.GetDisplayName() != DisplayName)
            {
                return false;
            }

            // Is there still space on the stack
            if (StackSize >= instance.GetMaxStackSize())
            {
                return false;
            }

            // Okay, worked
            stackS++;
            return true;
        }

        /// <summary>
        /// Removes an item from the stack.
        /// </summary>
        /// <returns>True if the entry should be destroyed.</returns>
        public bool Remove()
        {
            stackS--;
            return StackSize <= 0;
        }

        /// <summary>
        /// Called when the player wants to use an item
        /// </summary>
        /// <param name="entity">the entity</param>
        /// <returns>True if the instance can be destroyed</returns>
        public bool Use(GameObject entity)
        {
            if (instance.Use(entity))
            {
                return Remove();
            }

            return false;
        }

        /// <summary>
        /// returns texture
        /// </summary>
        /// <returns>the texture</returns>
        public Texture2D GetDrawable()
        {
            if (!texture)
            {
                texture = Inventory.GetTextureForItem(DisplayName);
            }
            return texture;
        }
    }

    /// <summary>
    /// The list of items in this inventory
    /// </summary>
    public ArrayList items;

    /// <summary>
    /// Sets the inventory to visible
    /// </summary>
    private bool visible;

    /// <summary>
    /// item collection sound
    /// </summary>
    private AudioClip collectionSound;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Awake()
    {
        items = new ArrayList();
        collectionSound = (AudioClip)Resources.Load("CollectItem");
    }

    /// <summary>
    /// Called for every frame
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            // Only open if not some other gui is already opened
            if ((!visible && Game.isMenuOpen) || Game.IsCutscene || Game.GetIgor() == null || !Game.GetIgorComponent().IsAlive())
            {
                return;
            }

            Game.isMenuOpen = !Game.isMenuOpen;
            Screen.showCursor = Game.isMenuOpen; /* Needed, since we don't pause the game for the inventory */
            visible = !visible;

            if (visible)
            {
                scrollPos = new Vector2();
            }
        }
    }

    /// <summary>
    /// Called when the inventory holder collides with an object
    /// </summary>
    /// <param name="other">The potential item we collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        IInvItem item = null;
        MonoBehaviour[] potentialItems = other.GetComponents<MonoBehaviour>();

        // Try to find Item implementations
        foreach (MonoBehaviour potentialItem in potentialItems)
        {
            if (potentialItem is BaseItem)
            {
                // We found an Item implementation
                item = ((BaseItem)potentialItem).PickUp();
                break;
            }
        }

        // Does not seem to be an item
        if (item == null)
        {
            return;
        }

        audio.PlayOneShot(collectionSound);
        // Insert it into the inventory
        InsertItem(item);
    }

    /// <summary>
    /// Sets the visibility of the inventory
    /// </summary>
    /// <param name="show">True for visible, false for invisible</param>
    public void SetShow(bool show)
    {
        visible = show;
    }

    /// <summary>
    /// Called when the GUI should be drawn
    /// </summary>
    public void OnGUI()
    {
        // Is the inventory visible
        if (visible)
        {
            // Set the style for item buttons (may have changed)
            GUI.skin.button.imagePosition = ImagePosition.ImageLeft;
            GUI.skin.button.alignment = TextAnchor.UpperRight;
            GUI.skin.button.fontSize = 15;

            // Set the style for the item names
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            // Set the style for the caption
            GUI.skin.box.fontSize = 20;

            // And draw it
            DrawInventory();
        }
    }

    /// <summary>
    /// Inserts the item into the inventory.
    /// </summary>
    /// <param name="item">The item to be inserted</param>
    private void InsertItem(IInvItem item)
    {
        Texture2D texture = GetTextureForItem(item.GetDisplayName());
        MessageBoard.AddMessage("Picked up " + item.GetDisplayName(), texture);

        // Try to add the item to one of the stacks
        foreach (InventoryItem ii in items)
        {
            // Try to stack it
            if (ii.Stack(item))
            {
                return;
            }
        }

        // Add a new stack
        items.Add(new InventoryItem(item, 1));
    }

    /// <summary>
    /// Scroll position of the inventory
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// The position of the item that is selected by the controller
    /// </summary>
    private int controllerPos;

    /// <summary>
    /// The last mouse position
    /// Used to detect mouse movements
    /// </summary>
    private Vector3 mousePos;

    /// <summary>
    /// Cooldown for controller input
    /// </summary>
    private float cooldown;

    /// <summary>
    /// Draws the inventory on the screen
    /// </summary>
    private void DrawInventory()
    {
        // Calculate the width of the inventory
        int vwidth = (int)(Screen.width * 0.4);
        if (vwidth < 175)
        {
            vwidth = 175;
        }

        // Calculate the vertical start of the inventory
        int vleft = (Screen.width - vwidth) / 2;

        // Calculate the number of columns of the inventory
        int cols = (vwidth - 25) / 150;

        // Show the background box (and caption)
        GUI.Box(new Rect(vleft, 10, vwidth, Screen.height - 115f), scrollPos.y == 0f ? "Inventory" : "");

        // Scrollolol
        scrollPos = GUI.BeginScrollView(new Rect(vleft, 10, vwidth, Screen.height - 120), scrollPos, new Rect(0, 0, vwidth - 20, items.Count / cols * 150 + (items.Count % cols == 0 ? 25 : 175)));

        // Draw buttons
        for (int i = 0; i < items.Count; i++)
        {
            DrawButton(i, cols);
        }

        // We are done with the scrolling section (use the scroll wheel)
        GUI.EndScrollView(true);


        // Cooldown for actions
        if (cooldown > 0f)
        {
            cooldown = Mathf.Max(cooldown -= Time.deltaTime, 0f);
        }

        // Deactivate controller control on mouse movement or no items
        if (items.Count == 0 || mousePos != Input.mousePosition)
        {
            mousePos = Input.mousePosition;
            controllerPos = -1;
        }
        else
        {
            // Calculate new controller pos
            if (Input.GetAxis("Horizontal") > 0.1 && cooldown == 0f)
            {
                if (++controllerPos >= items.Count)
                {
                    controllerPos = 0;
                }
                cooldown = 1f;
            }
            else if (Input.GetAxis("Horizontal") < -0.1 && cooldown == 0f)
            {
                if (--controllerPos < 0)
                {
                    controllerPos = items.Count - 1;
                }
                cooldown = 1f;
            }

            if (Input.GetAxis("Vertical") > 0.1 && cooldown == 0f)
            {
                controllerPos += cols;
                if (controllerPos >= items.Count)
                {
                    controllerPos = 0;
                }
                cooldown = 1f;
            }
            else if (Input.GetAxis("Vertical") < -0.1 && cooldown == 0f)
            {
                controllerPos -= cols;
                if (controllerPos < 0)
                {
                    controllerPos = -1 * controllerPos % cols;
                }
                cooldown = 1f;
            }
        }

        // Focus selected button
        if (controllerPos != -1)
        {
            GUI.FocusControl(controllerPos.ToString());
            if (Input.GetButton("Jump") && cooldown == 0f)
            {
                if (((InventoryItem)items[controllerPos]).Use(gameObject))
                {
                    items.RemoveAt(controllerPos);
                }
                cooldown = 1f;
            }
        }
    }

    /// <summary>
    /// Draws a single inventory button
    /// </summary>
    /// <param name="num">number of the item in the items list</param>
    /// <param name="perRow">number per row</param>
    private void DrawButton(int num, int perRow)
    {
        // Calculate row and column
        int row = num / perRow;
        int col = num % perRow;

        // Calculate exact position
        int x = col * 150 + 25;
        int y = row * 150 + 50;

        // Get the item to show
        InventoryItem item = (InventoryItem)items[num];

        // Generate the stack number
        string stack = "";
        if (item.StackSize != 1)
        {
            stack = item.StackSize.ToString();
        }

        // Set the name for this element
        GUI.SetNextControlName(num.ToString());

        // Draw the button and check if it has been clicked
        Rect buttonPosition = new Rect(x, y, 100, 100);
        if (GUI.Button(buttonPosition, new GUIContent(stack, item.GetDrawable(), item.DisplayName)))
        {
            // Try to use item (returns true if the stack is empty)
            if (item.Use(gameObject))
            {
                items.RemoveAt(num);
            }
        }
        // possibly initiate drag & drop
        else if (Input.GetMouseButtonDown(0) && buttonPosition.Contains(Event.current.mousePosition))
        {
            DragNDrop.StartDrag(item.DisplayName, DragNDrop.MetaData.Source.INVENTORY, item.GetDrawable());
        }

        // If we hover this button, we show the label
        if (GUI.tooltip != "")
        {
            GUI.Label(new Rect(x, y - 25, 100, 25), GUI.tooltip);
            GUI.tooltip = "";
        }
    }


    /// <summary>
    /// clears the inventory
    /// </summary>
    public void InventoryClear()
    {
        this.items = new ArrayList();
    }

    /// <summary>
    /// for loading
    /// </summary>
    /// <param name="list">fill data</param>
    public void FillInventoryFromSaveData(ArrayList list)
    {
        items = list;
    }


    /// <summary>
    /// Finds out if the given item exists in the inventory
    /// </summary>
    /// <param name="itemName">The display name of the object to find.</param>
    /// <returns>True if it exists</returns>
    public bool HasItem(string itemName)
    {
        foreach (InventoryItem ii in items)
        {
            if (ii.DisplayName == itemName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Counts the amount of items of the given type in the inventory
    /// </summary>
    /// <param name="itemName">Name of the items to count</param>
    /// <returns>Number of the items of this type in the inventory</returns>
    public int CountItem(string itemName)
    {
        int count = 0;

        foreach (InventoryItem ii in items)
        {
            if (ii.DisplayName == itemName)
            {
                count += ii.stackS;
            }
        }

        return count;
    }

    /// <summary>
    /// Uses the item
    /// </summary>
    /// <param name="itemName">Name of the item to use</param>
    public void UseItem(string itemName)
    {
        InventoryItem remove = null;

        foreach (InventoryItem ii in items)
        {
            if (ii.DisplayName == itemName)
            {
                if (ii.Use(this.gameObject))
                {
                    remove = ii;
                    break;
                }
            }
        }

        // Remove the stack if it is empty
        if (remove != null)
        {
            items.Remove(remove);
        }
    }

    /// <summary>
    /// Removes an item for the inventory
    /// </summary>
    /// <param name="itemName">Name of the item to remove</param>
    public void RemoveItem(string itemName)
    {
        InventoryItem remove = null;

        foreach (InventoryItem ii in items)
        {
            if (ii.DisplayName == itemName)
            {
                if (ii.Remove())
                {
                    remove = ii;
                    break;
                }
            }
        }

        // Remove the stack if it is empty
        if (remove != null)
        {
            items.Remove(remove);
        }

    }

    /// <summary>
    /// Returns the list of items in this inventory
    /// </summary>
    /// <returns>A string array containing all inventory items in alphabetical order</returns>
    public string[] GetItemList()
    {
        ArrayList itemList = new ArrayList();

        foreach (InventoryItem ii in items)
        {
            if (!itemList.Contains(ii.DisplayName))
            {
                itemList.Add(ii.DisplayName);
            }
        }

        itemList.Sort();
        return (string[])itemList.ToArray();
    }
}
