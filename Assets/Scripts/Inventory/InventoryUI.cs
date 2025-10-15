using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int maxVisibleSlots = 10;

    private List<InventorySlotUI> _slots = new List<InventorySlotUI>();

    private void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryUI: No InventoryManager found in scene!");
            return;
        }

        InventoryManager.Instance.onInventoryUpdated += RefreshUI;
        InventoryManager.Instance.onItemSelected += HighlightSelectedSlot;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryUpdated -= RefreshUI;
            InventoryManager.Instance.onItemSelected -= HighlightSelectedSlot;
        }
    }

    private void Update()
    {
        HandleScrollSelection();
    }

    /// <summary>
    /// Scroll wheel selection handler.
    /// Scroll down → move right (next)
    /// Scroll up → move left (previous)
    /// </summary>
    private void HandleScrollSelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // ✅ Inverted scroll direction for natural feel
            int direction = scroll < 0 ? 1 : -1;

            InventoryManager.Instance.ScrollSelection(direction);
            HighlightSelectedSlot(InventoryManager.Instance.GetCurrentItem());
        }
    }

    /// <summary>
    /// Rebuilds UI but preserves previously selected slot.
    /// </summary>
    public void RefreshUI()
    {
        // ✅ Preserve currently selected item
        var previouslySelected = InventoryManager.Instance.GetCurrentItem();

        ClearSlots();

        var items = InventoryManager.Instance.GetAllItems();
        int count = Mathf.Min(items.Count, maxVisibleSlots);

        for (int i = 0; i < count; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotParent);
            var slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUI.SetupSlot(items[i]);
            _slots.Add(slotUI);
        }

        // ✅ Reapply previous selection if still valid
        if (previouslySelected != null && items.Contains(previouslySelected))
        {
            HighlightSelectedSlot(previouslySelected);
            InventoryManager.Instance.SelectItemByIndex(items.IndexOf(previouslySelected));
        }
        else if (_slots.Count > 0 && InventoryManager.Instance.GetCurrentItem() == null)
        {
            InventoryManager.Instance.SelectItemByIndex(0);
            HighlightSelectedSlot(InventoryManager.Instance.GetCurrentItem());
        }
    }

    /// <summary>
    /// Clears all slots before rebuild.
    /// </summary>
    private void ClearSlots()
    {
        foreach (var slot in _slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        _slots.Clear();
    }

    /// <summary>
    /// Updates slot highlights.
    /// </summary>
    private void HighlightSelectedSlot(InventorySlot selectedSlot)
    {
        foreach (var slotUI in _slots)
        {
            if (slotUI == null) continue;
            slotUI.SetSelected(slotUI.RepresentedSlot == selectedSlot);
        }
    }
}
