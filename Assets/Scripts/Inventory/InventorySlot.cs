using System;

[Serializable]
public class InventorySlot
{
    public ItemDataSO itemData;
    public int quantity;

    public InventorySlot(ItemDataSO data, int qty = 1)
    {
        itemData = data;
        quantity = qty;
    }
}
