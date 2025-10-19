using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Base class for all crafting-related UI elements (e.g., buttons, ingredient boxes).
/// Handles shared UI functionality: icon, text, background, colors, and selection states.
/// </summary>
public abstract class CraftingUIElementBase : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TMP_Text labelText;
    [SerializeField] protected Image background;

    [Header("Colors")]
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color selectedColor = new(0.3f, 0.8f, 0.4f);
    [SerializeField] protected Color insufficientColor = Color.red;
    [SerializeField] protected Color lockedColor = Color.gray;

    protected bool isSelected = false;

    /// <summary>
    /// Sets the icon image for this UI element.
    /// </summary>
    protected void SetIcon(Sprite sprite)
    {
        if (iconImage == null) return;
        iconImage.sprite = sprite;
        iconImage.color = sprite ? Color.white : new Color(1, 1, 1, 0.25f);
    }

    /// <summary>
    /// Sets the main label text.
    /// </summary>
    protected void SetLabel(string text, Color? colorOverride = null)
    {
        if (labelText == null) return;
        labelText.text = text;
        labelText.color = colorOverride ?? normalColor;
    }

    /// <summary>
    /// Highlights or unhighlights this element.
    /// </summary>
    public virtual void SetSelected(bool selected)
    {
        isSelected = selected;
        if (background)
            background.color = selected ? selectedColor : normalColor;
    }

    /// <summary>
    /// For updating this UI element (to be implemented by subclasses).
    /// </summary>
    public abstract void Refresh();
}
