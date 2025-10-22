using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds cargo for a single vehicle instance. Attach to the vehicle prefab/gameobject.
/// Uses your existing InventorySlot class as container entries.
/// </summary>
public class VehicleCargo : MonoBehaviour
{
    [SerializeField] private VehicleDataSO vehicleData;
    [SerializeField] private List<InventorySlot> cargoSlots = new List<InventorySlot>();

    private float currentCargoWeight;

    public event Action OnCargoChanged;

    public VehicleDataSO VehicleData => vehicleData;

    private void Reset()
    {
        // initialize list to capacity in editor when adding component
        cargoSlots = new List<InventorySlot>(vehicleData != null ? vehicleData.cargoSlots : 0);
    }

    private void Awake()
    {
        // Ensure cargoSlots list has correct number of entries (empty slots represented by null or InventorySlot with null item)
        if (cargoSlots == null) cargoSlots = new List<InventorySlot>();

        // Do not pre-fill with dummy InventorySlot objects; we will treat null as empty
    }

    /// <summary>
    /// Tries to add 'quantity' of 'item' to this cargo. Will try to stack first, then create new slot.
    /// Returns true if at least one of the requested quantity was added; false if nothing could be added.
    /// </summary>
    public bool AddCargo(ItemDataSO item, int quantity)
    {
        if (item == null || quantity <= 0 || vehicleData == null) return false;

        float addedWeight = item.weight * quantity;
        if (currentCargoWeight + addedWeight > vehicleData.cargoCapacity)
        {
            UIManager.Instance?.ShowMessage("❌ Cargo hold full!");
            return false;
        }

        int remaining = quantity;
        // First, try to fill existing stacks
        for (int i = 0; i < cargoSlots.Count && remaining > 0; i++)
        {
            var slot = cargoSlots[i];
            if (slot != null && slot.itemData == item && slot.quantity < item.maxStack)
            {
                int space = item.maxStack - slot.quantity;
                int toAdd = Mathf.Min(space, remaining);
                slot.quantity += toAdd;
                remaining -= toAdd;
                currentCargoWeight += item.weight * toAdd;
            }
        }

        // Create or append new slots if we still have quantity and space
        while (remaining > 0 && cargoSlots.Count < vehicleData.cargoSlots)
        {
            int toAdd = Mathf.Min(item.maxStack, remaining);
            cargoSlots.Add(new InventorySlot(item, toAdd));
            remaining -= toAdd;
            currentCargoWeight += item.weight * toAdd;
        }

        // If remaining > 0 but no slots left, we added some but not all. That's acceptable; caller should handle leftover
        if (quantity - remaining > 0)
        {
            OnCargoChanged?.Invoke();
            UIManager.Instance?.ShowMessage($"📦 Loaded {quantity - remaining}x {item.itemName}");
            return true;
        }

        UIManager.Instance?.ShowMessage("❌ Could not load cargo (maybe full)");
        return false;
    }

    /// <summary>
    /// Remove 'quantity' of 'item' from cargo. Returns true if removal succeeded (partial removal allowed).
    /// </summary>
    public bool RemoveCargo(ItemDataSO item, int quantity)
    {
        if (item == null || quantity <= 0) return false;

        int remaining = quantity;

        // iterate slots and remove
        for (int i = 0; i < cargoSlots.Count && remaining > 0; i++)
        {
            var slot = cargoSlots[i];
            if (slot == null || slot.itemData != item) continue;

            int toRemove = Mathf.Min(slot.quantity, remaining);
            slot.quantity -= toRemove;
            remaining -= toRemove;
            currentCargoWeight -= item.weight * toRemove;

            if (slot.quantity <= 0)
            {
                cargoSlots.RemoveAt(i);
                i--; // adjust index after removal
            }
        }

        if (quantity - remaining > 0)
        {
            OnCargoChanged?.Invoke();
            UIManager.Instance?.ShowMessage($"📦 Unloaded {quantity - remaining}x {item.itemName}");
            return true;
        }

        return false;
    }

    public IReadOnlyList<InventorySlot> GetCargoSlots() => cargoSlots.AsReadOnly();
    public float GetCurrentCargoWeight() => currentCargoWeight;
    public int GetMaxSlots() => vehicleData != null ? vehicleData.cargoSlots : cargoSlots.Count;
    public float GetMaxCapacity() => vehicleData != null ? vehicleData.cargoCapacity : 0f;

    /// <summary>
    /// Helper: check whether the full quantity can be added (based on weight & slots).
    /// Does not modify cargo.
    /// </summary>
    public bool CanAddFully(ItemDataSO item, int quantity)
    {
        if (item == null) return false;
        float addedWeight = item.weight * quantity;
        if (currentCargoWeight + addedWeight > GetMaxCapacity()) return false;

        // check slot space (rough check)
        int remaining = quantity;
        for (int i = 0; i < cargoSlots.Count && remaining > 0; i++)
        {
            var slot = cargoSlots[i];
            if (slot != null && slot.itemData == item)
            {
                int space = item.maxStack - slot.quantity;
                remaining -= Mathf.Min(space, remaining);
            }
        }

        int freeSlots = Math.Max(0, GetMaxSlots() - cargoSlots.Count);
        int maxNew = freeSlots * item.maxStack;
        return remaining <= maxNew;
    }
}
