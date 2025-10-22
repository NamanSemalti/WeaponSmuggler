using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : BaseSlotUI
{
    private InventorySlot representedSlot;
    public InventorySlot RepresentedSlot => representedSlot;

    public void SetupSlot(InventorySlot slot)
    {
        representedSlot = slot;
        base.SetupSlot(slot?.itemData, slot?.quantity ?? 0);
    }

    public ItemDataSO GetItem()
    {
        return representedSlot != null ? representedSlot.itemData : null;
    }

    public int GetQuantity()
    {
        return representedSlot != null ? representedSlot.quantity : 0;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        // Check if player dragged something
        var draggedItem = UIDragIcon.Instance?.CurrentItem;
        int draggedQuantity = UIDragIcon.Instance?.CurrentQuantity ?? 0;
        if (draggedItem == null || draggedQuantity <= 0) return;

        // Case 1: Empty slot → add item directly
        if (representedSlot == null || representedSlot.itemData == null)
        {
            bool added = InventoryManager.Instance.AddItem(draggedItem, draggedQuantity);
            if (added)
            {
                UIManager.Instance?.ShowMessage($"✅ Added {draggedQuantity}x {draggedItem.itemName}");
                UIDragIcon.Instance.HideImmediate();
            }
            return;
        }

        // Case 2: Same item → stack up
        if (representedSlot.itemData == draggedItem)
        {
            int availableSpace = draggedItem.maxStack - representedSlot.quantity;
            int toAdd = Mathf.Min(availableSpace, draggedQuantity);

            representedSlot.quantity += toAdd;
            InventoryManager.Instance.onInventoryUpdated?.Invoke();
            UIManager.Instance?.ShowMessage($"📦 Stacked {toAdd}x {draggedItem.itemName}");
            UIDragIcon.Instance.HideImmediate();
            return;
        }

        // Case 3: Different item → swap (optional)
        var tempItem = representedSlot.itemData;
        var tempQty = representedSlot.quantity;

        representedSlot.itemData = draggedItem;
        representedSlot.quantity = draggedQuantity;

        // Optionally, give back the swapped item to player or vehicle
        InventoryManager.Instance.AddItem(tempItem, tempQty);

        InventoryManager.Instance.onInventoryUpdated?.Invoke();
        UIManager.Instance?.ShowMessage($"🔄 Swapped {tempItem.itemName} with {draggedItem.itemName}");
        UIDragIcon.Instance.HideImmediate();
    }
}
