using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI element that represents a single vehicle cargo slot. Inherits BaseSlotUI.
/// </summary>
public class VehicleSlotUI : BaseSlotUI
{
    private VehicleCargo linkedCargo;
    private int slotIndex = -1;
    private VehicleCargo boundCargo;
    /// <summary>
    /// Bind to cargo + slot index. Call this when instantiating the slot prefab in VehicleUI.
    /// </summary>
    public void Bind(VehicleCargo cargo, int index)
    {
        linkedCargo = cargo;
        slotIndex = index;
        boundCargo = cargo;
        Refresh();
        if (linkedCargo != null)
            linkedCargo.OnCargoChanged += Refresh;
    }

    private void OnDestroy()
    {
        if (linkedCargo != null)
            linkedCargo.OnCargoChanged -= Refresh;
    }

    /// <summary>
    /// Re-reads the bound slot and updates visuals.
    /// </summary>
    public void Refresh()
    {
        if (linkedCargo == null || slotIndex < 0)
        {
            ClearSlot();
            return;
        }

        var slots = linkedCargo.GetCargoSlots();
        if (slotIndex >= slots.Count)
        {
            ClearSlot();
            return;
        }

        var invSlot = slots[slotIndex];
        if (invSlot == null || invSlot.itemData == null)
        {
            ClearSlot();
            return;
        }

        base.SetupSlot(invSlot.itemData, invSlot.quantity);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        var icon = UIDragIcon.Instance;
        if (icon == null || icon.CurrentItem == null)
        {
            UIDragIcon.Instance?.HideAfterDelay();
            return;
        }

        ItemDataSO item = icon.CurrentItem;
        int amount = Mathf.Max(1, icon.CurrentQuantity); // ensure at least 1

        if (boundCargo == null)
        {
            UIManager.Instance?.ShowMessage("No vehicle linked");
            icon.HideAfterDelay();
            return;
        }

        // Try to add amount to vehicle cargo. AddCargo should accept partial or full addition.
        bool added = boundCargo.AddCargo(item, amount);

        if (added)
        {
            // remove from player inventory the exact amount
            InventoryManager.Instance.RemoveItem(item, amount);

            UIManager.Instance?.ShowMessage($"✅ Loaded {amount}x {item.itemName}");
        }
        else
        {
            UIManager.Instance?.ShowMessage($"❌ Could not load {amount}x {item.itemName} (capacity/full)");
        }

        // finally hide icon
        icon.HideAfterDelay();
    }


}
