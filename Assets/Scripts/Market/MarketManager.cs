using UnityEngine;
using System;

public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }

    [Header("Current Buyer")]
    public BuyerDataSO currentBuyer;

    public static event Action OnMarketUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public float CalculateSellPrice(ItemDataSO item)
    {
        if (currentBuyer == null) return 0f;

        float baseValue = item.baseValue;
        float dynamicValue = baseValue * currentBuyer.baseDemandMultiplier;
        float heatPenalty = 1f - HeatReputationManager.Instance.GetHeatPenalty();
        return Mathf.Round(dynamicValue * heatPenalty);
        // return dynamicValue;
    }

    public void SellItem(ItemDataSO item, int quantity)
    {
        float pricePerItem = CalculateSellPrice(item);
        float totalPrice = pricePerItem * quantity;

        // Remove items from inventory
        InventoryManager.Instance.RemoveItem(item, quantity);

        // Add money to player
        PlayerEconomy.Instance.AddMoney(totalPrice);

        // Adjust heat and reputation
        HeatReputationManager.Instance.AddHeat(currentBuyer.riskMultiplier);
        HeatReputationManager.Instance.AddReputation(currentBuyer.isLegal ? 1 : 2);

        UIManager.Instance.ShowMessage($"💰 Sold {quantity}x {item.itemName} for ${totalPrice:N0}");

        OnMarketUpdated?.Invoke();
    }
}
