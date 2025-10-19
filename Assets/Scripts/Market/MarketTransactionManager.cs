using UnityEngine;
using System.Collections.Generic;

public class MarketTransactionManager : MonoBehaviour
{
    public static MarketTransactionManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ProcessTrade(BuyerDataSO buyer, List<BuyerSlotUI> buyerSlots)
    {
        float totalEarned = 0f;

        foreach (var slot in buyerSlots)
        {
            var data = slot.GetRequestData();
            int delivered = slot.GetDeliveredQuantity();
            totalEarned += delivered * data.offeredPricePerUnit;
        }

        PlayerEconomy.Instance.AddMoney(totalEarned);
        HeatReputationManager.Instance.AddReputation(buyer.isIllegal ? 3 : 1);
        HeatReputationManager.Instance.AddHeat(buyer.isIllegal ? 5 : 1);

        UIManager.Instance.ShowMessage($"✅ Trade complete! Earned ${totalEarned:N0}");
    }
}
