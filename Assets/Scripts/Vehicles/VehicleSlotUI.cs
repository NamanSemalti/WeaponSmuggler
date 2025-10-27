using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text qtyText;
    [SerializeField] private Image highlightBorder; // outline for highlight

    private ItemDataSO currentItem;
    private int currentQuantity;
    public VehicleCargo LinkedCargo { get; private set; }

    public void Bind(VehicleCargo cargo)
    {
        LinkedCargo = cargo;
    }

    public void SetupSlot(ItemDataSO item, int quantity)
    {
        currentItem = item;
        currentQuantity = quantity;

        if (item == null)
        {
            icon.enabled = false;
            qtyText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;
        qtyText.text = quantity.ToString();
    }

    public ItemDataSO GetItem() => currentItem;
    public int GetQuantity() => currentQuantity;

    public void SetHighlighted(bool active)
    {
        if (highlightBorder != null)
            highlightBorder.enabled = active;
        if (active)
        {
            if (currentItem != null)
                UIManager.Instance.ShowInteractionMessage(currentItem.itemName);
        }
        else UIManager.Instance.HideInteractionMessage();
    }
}
