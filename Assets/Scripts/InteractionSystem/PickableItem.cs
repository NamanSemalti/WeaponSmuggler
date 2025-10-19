using UnityEngine;

public class PickableItem : Interactable
{
    [Header("Item Settings")]
    [SerializeField] private ItemDataSO itemDataSO;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private AudioClip pickupSound;

    [Header("Item Quantity")]
    [SerializeField, Range(1, 99)] private int quantity = 1;

    private bool _isPickedUp = false;

    protected override void OnInteract()
    {
        if (_isPickedUp) return;
        _isPickedUp = true;
        bool added = InventoryManager.Instance.AddItem(itemDataSO, quantity);
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
