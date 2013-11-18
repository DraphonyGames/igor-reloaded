using UnityEngine;
using System.Collections;

/// <summary>
/// the drag and drop system
/// </summary>
public class DragNDrop : MonoBehaviour
{
    #region StaticInterface

    /// <summary>
    /// the only ring
    /// </summary>
    private static DragNDrop dragNDropInstance;

    /// <summary>
    /// returns something
    /// </summary>
    /// <returns>it is the singleton!</returns>
    public static DragNDrop GetInstance()
    {
        if (dragNDropInstance == null)
        {
            GameObject prefab = (GameObject)Resources.Load("DragNDropPrefab");
            if (!prefab)
            {
                Debug.LogError("Could not load drag&drop prefab!");
                return null;
            }
            dragNDropInstance = (DragNDrop)((GameObject)Instantiate(prefab)).GetComponent<DragNDrop>();
        }
        return dragNDropInstance;
    }

    /// <summary>
    /// starts dragging
    /// </summary>
    /// <param name="info">the name</param>
    /// <param name="source">the source</param>
    /// <param name="texture">some picture</param>
    public static void StartDrag(string info, MetaData.Source source, Texture2D texture)
    {
        DragNDrop dragNDrop = GetInstance();
        dragNDrop.metaData = new MetaData(info, source, texture);
    }

    /// <summary>
    /// returns null or MetaData in case of drop
    /// </summary>
    /// <returns>the meta data</returns>
    public static MetaData GetDrop()
    {
        DragNDrop dragNDrop = GetInstance();
        if (dragNDrop.metaData == null)
        {
            return null;
        }
        if (dragNDrop.metaData.isActive == false)
        {
            return null;
        }
        if (dragNDrop.metaData.age != 1)
        {
            return null;
        }
        return dragNDrop.metaData;
    }

    #endregion
    /// <summary>
    /// drag and drop meta data
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// specifies the source of the drag and drop event
        /// </summary>
        public enum Source
        {
            /// <summary>
            /// Come from somewhere else.
            /// </summary>
            OTHER,
            /// <summary>
            /// Comes from the skill tree.
            /// </summary>
            SKILL_TREE,
            /// <summary>
            /// Comes from the inventory.
            /// </summary>
            INVENTORY
        }
        /// <summary>
        /// the info
        /// </summary>
        public string info;

        /// <summary>
        /// where from
        /// </summary>
        public Source source;

        /// <summary>
        /// the image
        /// </summary>
        public Texture2D drawingTexture;

        /// <summary>
        /// some coordinates
        /// </summary>
        public float x, y;

        /// <summary>
        /// drop events might die of old age
        /// </summary>
        public int age = 0;

        /// <summary>
        /// drag events only become active after the mouse moved for a certain time
        /// </summary>
        public bool isActive = false;

        /// <summary>
        /// size of the texture. Don't set (it's used for fading)
        /// </summary>
        public float size = 0f;

        /// <summary>
        /// a constructor
        /// </summary>
        /// <param name="_info">some info</param>
        /// <param name="_source">random source</param>
        /// <param name="_texture">what is this</param>
        public MetaData(string _info, Source _source, Texture2D _texture)
        {
            info = _info;
            source = _source;
            drawingTexture = _texture;
            x = Input.mousePosition.x;
            y = Screen.height - Input.mousePosition.y;
        }
    }

    /// <summary>
    /// some meta data
    /// </summary>
    private MetaData metaData = null;

    /// <summary>
    /// Unity Update
    /// </summary>
    private void Update()
    {
        // no draggin' goin' on?
        if (metaData == null)
        {
            return;
        }

        const int MOUSE_BUTTON = 0;

        if (!Input.GetMouseButton(MOUSE_BUTTON))
        {
            // let drop event die..
            if (++metaData.age > 2)
            {
                metaData = null;
            }
            return;
        }

        float x = Input.mousePosition.x;
        float y = Screen.height - Input.mousePosition.y;

        if (!metaData.isActive)
        {
            // activate the item only when a certain time/distance has passed
            float distanceVector = Mathf.Sqrt(Mathf.Pow(metaData.x - x, 2f) + Mathf.Pow(metaData.y - y, 2f));
            if (distanceVector > 10f)
            {
                metaData.isActive = true;
            }
            else
            {
                return;
            }
        }

        metaData.x = x;
        metaData.y = y;
    }

    /// <summary>
    /// Unity Callback
    /// </summary>
    private void OnGUI()
    {
        if (metaData == null || !metaData.isActive)
        {
            return;
        }

        // fade in/out
        if (metaData.age == 0 && metaData.size < 1f)
        {
            metaData.size += 0.1f;
        }
        else if (metaData.age > 1 && metaData.size > 0f)
        {
            metaData.size *= 0.5f;
        }

        float stdSize = 64f;
        float size = stdSize * metaData.size;
        GUI.DrawTexture(new Rect(metaData.x - size * 0.5f, metaData.y - size * 0.5f, size, size), metaData.drawingTexture);
        GUI.Label(new Rect(metaData.x - size * 0.5f, metaData.y + size * 0.5f, size, size * 0.5f), metaData.info);
    }
}
