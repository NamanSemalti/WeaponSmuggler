using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 30;
    [SerializeField] private float maxWeight = 50f;

    [Header("Debug View")]
    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();
    [SerializeField] private int currentSelectedIndex = -1; // ✅ new
    [SerializeField] private InventorySlot currentInventoryItem;  // ✅ New field
    public System.Action onInventoryUpdated;
    public System.Action<InventorySlot> onItemSelected;  // ✅ New event
    // ✅ This event notifies any UI or system that the inventory changed
    public static event Action onInventoryChanged;

    private float currentWeight;
    public InventorySlot GetCurrentItem()
    {
        if (currentSelectedIndex < 0 || currentSelectedIndex >= inventory.Count)
            return null;

        return inventory[currentSelectedIndex];
    }
    public int GetCurrentItemIndex()
    {
        if (currentSelectedIndex < 0 || currentSelectedIndex >= inventory.Count)
            return -1;
        return currentSelectedIndex;
    }
    public int GetItemQuantity(ItemDataSO itemData)
    {
        int total = 0;
        foreach (var slot in inventory)
        {
            if (slot.itemData == itemData)
                total += slot.quantity;
        }
        return total;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddItem(ItemDataSO itemData, int quantity = 1)
    {
        if (itemData == null || quantity <= 0) return false;

        float addedWeight = itemData.weight * quantity;
        if (currentWeight + addedWeight > maxWeight)
        {
            UIManager.Instance?.ShowMessage("❌ Inventory full (too heavy)");
            return false;
        }

        int remainingQuantity = quantity;

        // 1️⃣ Try to fill existing stacks first
        foreach (var slot in inventory)
        {
            if (slot.itemData == itemData && slot.quantity < itemData.maxStack)
            {
                int availableSpace = itemData.maxStack - slot.quantity;
                int toAdd = Mathf.Min(availableSpace, remainingQuantity);

                slot.quantity += toAdd;
                remainingQuantity -= toAdd;
                currentWeight += itemData.weight * toAdd;

                if (remainingQuantity <= 0)
                {
                    onInventoryUpdated?.Invoke();
                    UIManager.Instance?.ShowMessage($"+ {quantity}x {itemData.itemName}");
                    return true;
                }
            }
        }

        // 2️⃣ If still remaining, try creating new slots
        while (remainingQuantity > 0)
        {
            if (inventory.Count >= maxSlots)
            {
                UIManager.Instance?.ShowMessage("❌ Inventory full (no free slots)");
                return false;
            }

            int toAdd = Mathf.Min(itemData.maxStack, remainingQuantity);
            inventory.Add(new InventorySlot(itemData, toAdd));
            currentWeight += itemData.weight * toAdd;
            remainingQuantity -= toAdd;
        }

        // 3️⃣ Done — trigger UI update
        onInventoryUpdated?.Invoke();
        UIManager.Instance?.ShowMessage($"+ {quantity}x {itemData.itemName}");
        NotifyInventoryChanged(); // ✅ broadcast update
        return true;
    }
    public void SelectItemByIndex(int index)
    {
        if (inventory == null || inventory.Count == 0)
            return;

        if (index < 0 || index >= inventory.Count)
            return;

        currentSelectedIndex = index;
        var selectedSlot = inventory[index];
        onItemSelected?.Invoke(selectedSlot);

        UIManager.Instance?.ShowMessage($"🎯 Selected: {selectedSlot.itemData.itemName}");
    }
    public void ScrollSelection(int direction)
    {
        if (inventory.Count == 0) return;

        currentSelectedIndex += direction;

        if (currentSelectedIndex >= inventory.Count)
            currentSelectedIndex = 0;
        else if (currentSelectedIndex < 0)
            currentSelectedIndex = inventory.Count - 1;

        SelectItemByIndex(currentSelectedIndex);
    }


    public bool RemoveItem(ItemDataSO itemData, int quantity = 1)
    {
        InventorySlot slot = inventory.Find(s => s.itemData == itemData);
        if (slot == null) return false;

        slot.quantity -= quantity;
        currentWeight -= itemData.weight * quantity;

        if (slot.quantity <= 0)
            inventory.Remove(slot);

        UIManager.Instance?.ShowMessage($"Removed {itemData.itemName}");
        NotifyInventoryChanged(); // ✅ broadcast update
        return true;
    }

    public bool HasItem(string itemID)
    {
        return inventory.Exists(s => s.itemData.itemID == itemID);
    }

    public bool HasItem(ItemDataSO itemData)
    {
        return inventory.Exists(s => s.itemData == itemData);
    }

    public int GetItemCount(ItemDataSO itemData)
    {
        InventorySlot slot = inventory.Find(s => s.itemData == itemData);
        return slot != null ? slot.quantity : 0;
    }
    private void NotifyInventoryChanged()
    {
        onInventoryChanged?.Invoke();
    }
    public float GetCurrentWeight() => currentWeight;

    public List<InventorySlot> GetAllItems() => inventory;
}
