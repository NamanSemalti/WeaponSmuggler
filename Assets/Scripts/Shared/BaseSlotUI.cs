using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Base slot UI for inventory/vehicle/buyer slots.
/// Right-hold to select quantity (scroll to change). While right is held you may start a normal drag (left click) and the selected quantity will be used.
/// Normal drag (no right-hold selection) moves the full stack.
/// Drop targets should read UIDragIcon.Instance.CurrentItem and CurrentQuantity to know what to transfer.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("Common UI References")]
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TMP_Text quantityText;
    [SerializeField] protected Image rarityBorder;
    [SerializeField] protected Image background;
    [SerializeField] protected Image selectionHighlight;

    protected ItemDataSO currentItem;
    protected int currentQuantity;

    protected CanvasGroup canvasGroup;

    // Right-hold selection state
    private bool isRightHeld = false;           // true while right mouse is pressed down on this slot
    private bool isSelectingQuantity = false;   // true after right-down (we show UIDragIcon)
    private int selectedQuantity = 0;

    // Dragging state (we only allow one drag per pointer interaction)
    private bool isDragging = false;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        // Only poll scroll / cancel if user is actively selecting quantity on THIS slot
        if (!isSelectingQuantity) return;

        // scroll wheel: change selectedQuantity
        float scroll = Mouse.current?.scroll.ReadValue().y ?? 0f;
        if (Mathf.Abs(scroll) > 0.001f)
        {
            int dir = scroll > 0 ? 1 : -1;
            selectedQuantity = Mathf.Clamp(selectedQuantity + dir, 1, Mathf.Max(1, currentQuantity));
            UIManager.Instance?.ShowMessage($"Select quantity: {selectedQuantity}/{currentQuantity}");
            // update visible quantity text on icon
            if (UIDragIcon.Instance != null)
                UIDragIcon.Instance.Show(currentItem.icon, selectedQuantity);
        }

        // Cancel with ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelSelection();
        }
    }

    // -------------------------------
    // Setup & display
    // -------------------------------
    public virtual void SetupSlot(ItemDataSO item, int quantity = 0)
    {
        currentItem = item;
        currentQuantity = quantity;

        if (item == null)
        {
            ClearSlot();
            return;
        }

        if (itemIcon)
        {
            itemIcon.enabled = true;
            itemIcon.sprite = item.icon;
        }

        if (quantityText)
            quantityText.text = quantity > 1 ? quantity.ToString() : "";

        if (rarityBorder)
            rarityBorder.color = GetRarityColor(item.rarity);
    }

    protected virtual void ClearSlot()
    {
        if (itemIcon) itemIcon.enabled = false;
        if (quantityText) quantityText.text = "";
        if (selectionHighlight) selectionHighlight.enabled = false;

        currentItem = null;
        currentQuantity = 0;
    }

    public virtual void SetSelected(bool isSelected)
    {
        if (selectionHighlight)
            selectionHighlight.enabled = isSelected;
        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
    }

    protected virtual Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return new Color(0.7f, 0.7f, 0.7f);
            case Rarity.Uncommon: return Color.green;
            case Rarity.Rare: return Color.blue;
            case Rarity.Epic: return new Color(0.6f, 0.2f, 0.8f);
            case Rarity.Legendary: return new Color(1f, 0.6f, 0f);
            default: return Color.white;
        }
    }

    // -------------------------------
    // Pointer events: right-hold selection
    // -------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && currentItem != null && currentQuantity > 0)
        {
            // start selection mode
            isRightHeld = true;
            isSelectingQuantity = true;
            selectedQuantity = Mathf.Clamp(1, 1, currentQuantity); // initial 1 (you can change to currentQuantity if desired)
            // set icon logical item and quantity
            if (UIDragIcon.Instance != null)
            {
                UIDragIcon.Instance.SetDraggedItem(currentItem, selectedQuantity);
                UIDragIcon.Instance.Show(currentItem.icon, selectedQuantity);
            }
            UIManager.Instance?.ShowMessage($"Select quantity: {selectedQuantity}/{currentQuantity}");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // if releasing right button -> cancel selection or stop (if we are mid drag let OnEndDrag/OnDrop handle)
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightHeld = false;

            // If not currently dragging with left button, hide selection and icon.
            if (!isDragging)
                CancelSelection();
            else
            {
                // If dragging is ongoing, keep UIDragIcon (drag will end when user releases left click)
                // selection remains logically until drop completes
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // left or right click briefly: nothing extra here (we handle hold)
    }

    // Cancel selection helper
    private void CancelSelection()
    {
        isSelectingQuantity = false;
        selectedQuantity = 0;
        UIManager.Instance?.ShowMessage("");
        if (UIDragIcon.Instance != null)
            UIDragIcon.Instance.HideAfterDelay(0f);
    }

    // -------------------------------
    // Drag handling
    // -------------------------------
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // left-button drag always triggers OnBeginDrag; decide which quantity to use:
        if (currentItem == null) return;

        isDragging = true;

        // If user was right-holding selection and has a selectedQuantity, use that.
        if (isSelectingQuantity && selectedQuantity > 0)
        {
            // set UIDragIcon logic if not already set
            if (UIDragIcon.Instance != null)
            {
                UIDragIcon.Instance.SetDraggedItem(currentItem, selectedQuantity);
                UIDragIcon.Instance.Show(currentItem.icon, selectedQuantity);
            }

            // block raycasts on source slot to avoid immediate drop on itself
            if (canvasGroup) canvasGroup.blocksRaycasts = false;
            return;
        }

        // Default full-stack drag
        if (UIDragIcon.Instance != null)
        {
            UIDragIcon.Instance.SetDraggedItem(currentItem, currentQuantity);
            UIDragIcon.Instance.Show(currentItem.icon, currentQuantity);
        }

        if (canvasGroup) canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // nothing — UIDragIcon follows mouse
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // End drag: hide the icon after a short delay so drop handlers can read it
        isDragging = false;
        // Always hide selection state after drag finishes
        isSelectingQuantity = false;
        selectedQuantity = 0;

        if (UIDragIcon.Instance != null)
            UIDragIcon.Instance.HideAfterDelay(0.02f);

        if (canvasGroup) canvasGroup.blocksRaycasts = true;
    }

    // -------------------------------
    // Drop: implemented by subclasses (vehicle/buyer/inventory)
    // -------------------------------
    public abstract void OnDrop(PointerEventData eventData);

    // -------------------------------
    // Data getters for convenience
    // -------------------------------
    public ItemDataSO GetItem() => currentItem;
    public int GetQuantity() => currentQuantity;

    private void OnDestroy()
    {
        // Ensure icon hidden if slot removed mid-interaction
        if (UIDragIcon.Instance != null)
        {
            if (UIDragIcon.Instance.gameObject.activeInHierarchy)
                UIDragIcon.Instance.HideAfterDelay(0f);
            else
                UIDragIcon.Instance.HideImmediate();
        }
    }

}
