using UnityEngine;

public enum ItemType
{
    Resource,
    WeaponPart,
    Weapon,
    KeyItem,
    Consumable,
    Tool,
    Misc
}

[CreateAssetMenu(fileName = "NewItem", menuName = "WeaponSmuggler/Item Data", order = 0)]
public class ItemDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemID; // Unique internal ID
    public string itemName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;
    public GameObject worldPrefab;

    [Header("Stats")]
    public ItemType itemType;
    public int maxStack = 10;
    public float weight = 1f;
    public int baseValue = 100;
    public Rarity rarity = Rarity.Common;

    [Header("Special Flags")]
    public bool isQuestItem = false;
    public bool isKeyItem = false;
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
