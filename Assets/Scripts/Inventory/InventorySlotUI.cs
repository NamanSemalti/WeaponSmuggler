using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image selectionHighlight;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform rectTransform;
    private InventorySlot representedSlot;

    public InventorySlot RepresentedSlot => representedSlot;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    // ✅ Add these methods so drag/drop system works
    public ItemDataSO GetItem()
    {
        return representedSlot != null ? representedSlot.itemData : null;
    }

    public int GetQuantity()
    {
        return representedSlot != null ? representedSlot.quantity : 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        canvasGroup.blocksRaycasts = true;
    }

    public void SetupSlot(InventorySlot slot)
    {
        representedSlot = slot;

        if (slot == null || slot.itemData == null)
        {
            ClearSlot();
            return;
        }

        itemIcon.enabled = true;
        itemIcon.sprite = slot.itemData.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        rarityBorder.color = GetRarityColor(slot.itemData.rarity);

        if (selectionHighlight)
            selectionHighlight.enabled = false;
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight)
            selectionHighlight.enabled = isSelected;

        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
    }

    private void ClearSlot()
    {
        itemIcon.enabled = false;
        quantityText.text = "";
        if (selectionHighlight)
            selectionHighlight.enabled = false;
    }

    private Color GetRarityColor(Rarity rarity)
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
}
