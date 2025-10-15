using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "WeaponSmuggler/LootTable", order = 1)]
public class LootTableSO : ScriptableObject
{
    [SerializeField] private List<LootEntry> lootEntries = new List<LootEntry>();

    public LootEntry GetRandomLoot()
    {
        float totalWeight = 0f;
        foreach (var entry in lootEntries)
            totalWeight += entry.dropChance;

        float randomValue = Random.value * totalWeight;

        foreach (var entry in lootEntries)
        {
            if (randomValue < entry.dropChance)
                return entry;

            randomValue -= entry.dropChance;
        }

        return null;
    }

    public List<LootEntry> GetAllLoot()
    {
        return lootEntries;
    }
}

[System.Serializable]
public class LootEntry
{
    public string itemName;
    public GameObject prefab;     // Usually a PickableItem prefab
    [Range(0f, 100f)] public float dropChance = 25f; // Weighted chance
}
