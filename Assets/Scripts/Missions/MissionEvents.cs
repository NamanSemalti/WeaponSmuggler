// MissionEvents.cs
using System;

public static class MissionEvents
{
    // Delivery: item and amount delivered (to buyer/drop-off)
    public static Action<ItemDataSO, int> OnItemDelivered;

    // crafting: item and amount crafted
    public static Action<ItemDataSO, int> OnItemCrafted;

    // cargo loaded into vehicle
    public static Action<ItemDataSO, int> OnCargoLoaded;

    public static void TriggerItemDelivered(ItemDataSO item, int amount) => OnItemDelivered?.Invoke(item, amount);
    public static void TriggerItemCrafted(ItemDataSO item, int amount) => OnItemCrafted?.Invoke(item, amount);
    public static void TriggerCargoLoaded(ItemDataSO item, int amount) => OnCargoLoaded?.Invoke(item, amount);
}
