using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuyerSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image background;

    private BuyerRequest requestData;
    private int deliveredQuantity = 0;

    public void Setup(BuyerRequest request)
    {
        requestData = request;
        if (itemNameText)
            itemNameText.text = request.item.itemName;
        quantityText.text = $"0 / {request.requiredQuantity}";
        itemIcon.sprite = request.item.icon;
        background.color = Color.white;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<InventorySlotUI>();
        if (draggedItem == null) return;

        var item = draggedItem.GetItem();
        int amount = draggedItem.GetQuantity();

        if (item == requestData.item)
        {
            int accepted = Mathf.Min(amount, requestData.requiredQuantity - deliveredQuantity);
            deliveredQuantity += accepted;
            quantityText.text = $"{deliveredQuantity} / {requestData.requiredQuantity}";
            InventoryManager.Instance.RemoveItem(item, accepted);

            if (IsFilled())
                background.color = Color.green;
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
