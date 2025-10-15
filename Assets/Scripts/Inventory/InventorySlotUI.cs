using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private Image selectionHighlight;

    private InventorySlot representedSlot;

    public InventorySlot RepresentedSlot => representedSlot;

    /// <summary>
    /// Sets up slot with item data and quantity.
    /// </summary>
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

    /// <summary>
    /// Highlights or unhighlights this slot visually.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight)
            selectionHighlight.enabled = isSelected;

        // Optional visual feedback (slight scale pop)
        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
    }

    /// <summary>
    /// Clears this slot (no item).
    /// </summary>
    private void ClearSlot()
    {
        itemIcon.enabled = false;
        quantityText.text = "";
        if (selectionHighlight)
            selectionHighlight.enabled = false;
    }

    /// <summary>
    /// Rarity-based border color.
    /// </summary>
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
