using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// this is a static class for dropping items
/// example usage: ItemDropper.DropMicrochip(gameObject);
/// </summary>
public static class ItemDropper
{
    #region StaticDrop

    /// <summary>
    /// Microchip prefab to instantiate
    /// </summary>
    private static GameObject microchipPrefab;

    /// <summary>
    /// drops one microchip at a specified location
    /// </summary>
    /// <param name="transform">where to drop</param>
    public static void DropMicrochip(Transform transform)
    {
        Vector3 position = transform.position;
        if (microchipPrefab == null)
        {
            microchipPrefab = (GameObject)Resources.Load("XPchip");
        }
        GameObject.Instantiate(microchipPrefab, position + new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)), Quaternion.identity);
    }

    /// <summary>
    /// statically drops an item at a position, this will NOT cache the item
    /// note that the prefab still has to be in a "Resources" folder
    /// </summary>
    /// <param name="transform">where to drop</param>
    /// <param name="prefabName">what to drop</param>
    public static void DropItem(Transform transform, string prefabName)
    {
        GameObject prefab = (GameObject)Resources.Load(prefabName);
        if (!prefab)
        {
            Debug.LogError("Could not load prefab \"" + prefabName + "\"");
            return;
        }
        GameObject.Instantiate(prefab, transform.position + new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)), Quaternion.identity);
    }

    #endregion

    #region DynamicDrop

    /// <summary>
    /// defines the prefab names which are considered "common", those should be in a "Resources" folder!
    /// </summary>
    private static string[] commonItemNames = new string[] { "RepairKitPrefab", "EnergyCapsulePrefab" };

    /// <summary>
    /// s.a. only for "rare"
    /// </summary>
    private static string[] rareItemNames = new string[] { "MicrowavePrefab", "PrismPrefab", "Coil", "Fan", "Spring", "USBStickPrefab" };

    /// <summary>
    /// keeping track of not only the prefab but also the last drop etc.
    /// </summary>
    private class ItemDropData
    {
        /// <summary>
        /// which item
        /// </summary>
        public GameObject prefab = null;

        /// <summary>
        /// how recent is the last drop
        /// </summary>
        public float actuality = 0;

        /// <summary>
        /// a constructor
        /// </summary>
        /// <param name="_prefab">with a prefab</param>
        public ItemDropData(GameObject _prefab)
        {
            prefab = _prefab;
        }

        /// <summary>
        /// become less recent
        /// </summary>
        public void Degenerate()
        {
            actuality *= 0.9f;
        }
    }

    /// <summary>
    /// chances that can be adjusted to improve the dropping experience
    /// </summary>
    private const float CommonItemChance = 20f;

    /// <summary>
    /// see above
    /// </summary>
    private const float RareItemChance = 5 * 20f;

    /// <summary>
    /// contains items for "rare", "common"
    /// </summary>
    private static Dictionary<string, List<ItemDropData>> dropData = new Dictionary<string, List<ItemDropData>>();

    /// <summary>
    /// see above
    /// </summary>
    private static Dictionary<string, float> dynamicDropChances = new Dictionary<string, float>();

    /// <summary>
    /// drops an item of a specified type, respecting drop chance etc.
    /// </summary>
    /// <param name="transform">where to drop</param>
    /// <param name="type">what kind of item to drop</param>
    public static void DropItemOfType(Transform transform, string type)
    {
        // see whether the actual prefabs for that type have already been loaded..
        if (!dropData.ContainsKey(type))
        {
            dropData.Add(type, new List<ItemDropData>());
            List<ItemDropData> itemList = dropData[type];
            // load..
            string[] namesToLoad = commonItemNames;
            if (type == "rare")
            {
                namesToLoad = rareItemNames;
            }
            foreach (string name in namesToLoad)
            {
                GameObject prefab = (GameObject)Resources.Load(name);
                if (prefab == null)
                {
                    Debug.LogError("Could not load prefab \"" + name + "\" of type \"" + type + "\"");
                    continue;
                }
                itemList.Add(new ItemDropData(prefab));
            }
        }

        List<ItemDropData> itemDropDataList = dropData[type];
        if (itemDropDataList.Count == 0)
        {
            Debug.Log("Warning: item drop data list is empty for \"" + type + "\"");
            return;
        }

        // degenerate actualities so that items can be found again
        foreach (ItemDropData data in itemDropDataList)
        {
            data.Degenerate();
        }

        // figure out item to drop
        ItemDropData itemToDrop = null;
        float actualityThreshold = 1f;
        int failsafeMaxCount = 10000;

        while (--failsafeMaxCount > 0)
        {
            ItemDropData randomItem = itemDropDataList[Random.Range(0, itemDropDataList.Count)];
            float fairChance = randomItem.actuality - actualityThreshold;

            if (fairChance > Random.Range(0, 100f))
            {
                actualityThreshold *= 2f;
                continue;
            }
            itemToDrop = randomItem;
            break;
        }

        if (itemToDrop == null)
        {
            Debug.Log("Warning: couldn't figure out which item to drop for type \"" + type + "\"");
            return;
        }

        // make it a recent drop!
        itemToDrop.actuality = 100f;
        
        GameObject obj = (GameObject)GameObject.Instantiate(itemToDrop.prefab, transform.position, Quaternion.identity);

        // fix items dropping through ground..
        float offsetY = (obj.collider != null) ? obj.collider.bounds.size.y : 0f;
        obj.transform.Translate(0f, offsetY, 0f);
    }

    /// <summary>
    /// a microchip is worth 1
    /// </summary>
    /// <param name="transform">where to drop</param>
    /// <param name="value">how valuable the drop should be (currency: microchips)</param>
    public static void DynamicDrop(Transform transform, float value)
    {
        if (dynamicDropChances.Count == 0)
        {
            dynamicDropChances.Add("rare", 0f);
            dynamicDropChances.Add("common", 0f);
        }
        // this weird copying is done to be able to modify the collection while iterating over its keys..
        string[] keyCopy = new string[dynamicDropChances.Keys.Count];
        dynamicDropChances.Keys.CopyTo(keyCopy, 0);
        foreach (string key in keyCopy)
        {
            dynamicDropChances[key] += value;

            // roll die for drop
            float chance = 0f;
            switch (key)
            {
                case "rare":
                    chance = RareItemChance;
                    break;
                case "common":
                    chance = CommonItemChance;
                    break;
                default:
                    Debug.LogError("Missing item type");
                    break;
            }
            float fairChance = dynamicDropChances[key] - chance;

            if (fairChance < Random.Range(0f, 100f))
            {
                continue;
            }
            
            // generate loot!
            DropItemOfType(transform, key);
            // make sure to not only drop the ultimate sword of eternal destruction all the time
            dynamicDropChances[key] -= chance;
        }

        // aaand finally, drop microchips - like we were actually supposed to
        int microchipCount = (int)value;
        while (microchipCount-- > 0)
        {
            DropMicrochip(transform);
        }
    }

    #endregion
}
