using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleCargo : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private VehicleDataSO vehicleDataSO;
    [SerializeField] private Transform cargoTriggerPoint; // assign near door

    private List<InventorySlot> cargoSlots = new();
    private float currentWeight;

    public UnityEvent OnCargoUpdated;

    public int CargoSlots => vehicleDataSO.cargoSlots;
    public float CargoCapacity => vehicleDataSO.cargoCapacity;
    public System.Action OnInventoryChanged;

    private void Awake()
    {
        // Preallocate empty slots
        for (int i = 0; i < vehicleDataSO.cargoSlots; i++)
            cargoSlots.Add(new InventorySlot(null, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        // Early exit if no item visual
        if (!other.TryGetComponent(out ItemWorld itemVisual) || itemVisual.ItemData == null)
            return;

        int qty = 1;

        // Optional: if your PickableItem extends Interactable, you can skip double checks
        if (other.TryGetComponent(out PickableItem pickable))
            qty = pickable.GetQuantity;

        if (TryAddItem(itemVisual.ItemData, qty))
        {
            ObjectPoolManager.Instance.ReturnToPool(itemVisual.ItemData.handPoolKey, other.gameObject);
            UIManager.Instance?.ShowMessage($"📦 Loaded {qty}x {itemVisual.ItemData.itemName}");
        }
    }

    public bool TryAddItem(ItemDataSO item, int quantity)
    {
        if (item == null) return false;

        float addedWeight = item.weight * quantity;
        if (currentWeight + addedWeight > vehicleDataSO.cargoCapacity)
        {
            UIManager.Instance?.ShowMessage("❌ Cargo hold is full!");
            return false;
        }

        int remaining = quantity;

        // Try stacking first
        foreach (var slot in cargoSlots)
        {
            if (slot.itemData == item && slot.quantity < item.maxStack)
            {
                int space = item.maxStack - slot.quantity;
                int toAdd = Mathf.Min(space, remaining);
                slot.quantity += toAdd;
                currentWeight += toAdd * item.weight;
                remaining -= toAdd;
                if (remaining <= 0)
                {
                    OnCargoUpdated?.Invoke();
                    return true;
                }
            }
        }

        // Fill empty slots
        foreach (var slot in cargoSlots)
        {
            if (slot.itemData == null)
            {
                int toAdd = Mathf.Min(item.maxStack, remaining);
                slot.itemData = item;
                slot.quantity = toAdd;
                currentWeight += toAdd * item.weight;
                remaining -= toAdd;
                if (remaining <= 0)
                {
                    OnCargoUpdated?.Invoke();
                    return true;
                }
            }
        }

        OnCargoUpdated?.Invoke();
        return remaining <= 0;
    }
    /// <summary>
    /// Removes the given quantity of a specific item type from the vehicle’s cargo.
    /// </summary>
    public int RemoveMatchingItems(ItemDataSO item, int quantity)
    {
        if (item == null || quantity <= 0)
            return 0;

        int removed = 0;

        for (int i = 0; i < cargoSlots.Count && removed < quantity; i++)
        {
            var slot = cargoSlots[i];
            if (slot.itemData == item && slot.quantity > 0)
            {
                int toRemove = Mathf.Min(slot.quantity, quantity - removed);
                slot.quantity -= toRemove;
                removed += toRemove;
                currentWeight -= toRemove * item.weight;

                // Clear empty slots
                if (slot.quantity <= 0)
                    cargoSlots[i] = new InventorySlot(null, 0);
            }
        }

        if (removed > 0)
            OnCargoUpdated?.Invoke(); // refresh UI if any items were removed

        return removed;
    }

    public List<InventorySlot> GetCargoSlots() => cargoSlots;
    public float GetWeightPercent() => currentWeight / vehicleDataSO.cargoCapacity;
}
