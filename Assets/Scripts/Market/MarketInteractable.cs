using UnityEngine;

public class MarketInteractable : Interactable
{
    [SerializeField] private BuyerDataSO buyerData;

    protected override void OnInteract()
    {
        if (buyerData == null) return;
        MarketUI.Instance.OpenMarket(buyerData);
    }
}
