using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuyerSlotUI : BaseSlotUI
{
    [SerializeField] private TMP_Text itemNameText;
    // itemIcon and background come from BaseSlotUI fields

    private BuyerRequest requestData;
    private int deliveredQuantity = 0;

    public void Setup(BuyerRequest request)
    {
        requestData = request;
        base.SetupSlot(request.item, 0);
        if (itemNameText) itemNameText.text = request.item.itemName;
        if (quantityText) quantityText.text = $"0 / {request.requiredQuantity}";
        if (background) background.color = Color.white;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        var draggedIcon = UIDragIcon.Instance;
        if (draggedIcon == null || draggedIcon.CurrentItem == null) return;

        var item = draggedIcon.CurrentItem;
        int amount = draggedIcon.CurrentQuantity > 0 ? draggedIcon.CurrentQuantity : 1; // fallback


        if (item == requestData.item)
        {
            int accepted = Mathf.Min(amount, requestData.requiredQuantity - deliveredQuantity);
            deliveredQuantity += accepted;

            if (quantityText) quantityText.text = $"{deliveredQuantity} / {requestData.requiredQuantity}";

            InventoryManager.Instance.RemoveItem(item, accepted);

            if (IsFilled() && background) background.color = Color.green;
        }
        else
        {
            UIManager.Instance.ShowMessage($"❌ {item.itemName} not requested by this buyer!");
        }
    }

    public bool IsFilled() => deliveredQuantity >= requestData.requiredQuantity;
    public BuyerRequest GetRequestData() => requestData;
    public int GetDeliveredQuantity() => deliveredQuantity;
}
