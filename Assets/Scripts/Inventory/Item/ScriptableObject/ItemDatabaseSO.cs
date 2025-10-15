using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "WeaponSmuggler/Item Database", order = 1)]
public class ItemDatabaseSO : ScriptableObject
{
    [Header("Registered Items")]
    [SerializeField] private List<ItemDataSO> allItems = new List<ItemDataSO>();

    private Dictionary<string, ItemDataSO> _itemLookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        _itemLookup = new Dictionary<string, ItemDataSO>();

        foreach (var item in allItems)
        {
            if (item == null || string.IsNullOrEmpty(item.itemID))
            {
                Debug.LogWarning("ItemDatabase contains an invalid item or missing ID.");
                continue;
            }

            if (_itemLookup.ContainsKey(item.itemID))
            {
                Debug.LogWarning($"Duplicate item ID found in database: {item.itemID}");
                continue;
            }

            _itemLookup.Add(item.itemID, item);
        }
    }

    public ItemDataSO GetItemByID(string id)
    {
        if (_itemLookup == null || _itemLookup.Count == 0)
            BuildLookup();

        if (_itemLookup.TryGetValue(id, out ItemDataSO item))
            return item;

        Debug.LogWarning($"Item with ID '{id}' not found in database!");
        return null;
    }

    public List<ItemDataSO> GetAllItems() => allItems;
}
