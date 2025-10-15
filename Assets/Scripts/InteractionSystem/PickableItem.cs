using UnityEngine;

public class PickableItem : Interactable
{
    [Header("Item Settings")]
    [SerializeField] private string itemID; // unique identifier (for inventory integration later)
    [SerializeField] private string itemName = "Item";
    [SerializeField] private Sprite icon;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private AudioClip pickupSound;

    [Header("Item Quantity")]
    [SerializeField, Range(1, 99)] private int quantity = 1;

    private bool _isPickedUp = false;

    protected override void OnInteract()
    {
        if (_isPickedUp) return;

        _isPickedUp = true;

        ItemDataSO data = ItemDatabase.GetItemByID(itemID);
        bool added = InventoryManager.Instance.AddItem(data, quantity);

        if (added)
        {
            InteractionEvents.TriggerPickup(this);
            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (destroyOnPickup)
                Destroy(gameObject);
        }
        else
        {
            _isPickedUp = false; // Inventory full or add failed
        }
    }



    private void AddToInventory()
    {
        // Placeholder for inventory system
        // In MVP: can just log item pickup
        Debug.Log($"Picked up: {itemName} (x{quantity})");

        // Later: InventoryManager.Instance.AddItem(itemID, quantity);
    }

    // Optional highlight visual when focused
    protected override void OnFocus()
    {
        // Example: enable glow, outline, or change material
        // HighlightManager.Instance.HighlightObject(gameObject);
    }

    protected override void OnLoseFocus()
    {
        // Remove highlight
        // HighlightManager.Instance.RemoveHighlight(gameObject);
    }
}
