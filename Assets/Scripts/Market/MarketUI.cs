using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MarketUI : MonoBehaviour
{
    public static MarketUI Instance;

    [Header("References")]
    [SerializeField] private GameObject marketPanel;
    [SerializeField] private Transform buyerSlotParent;
    [SerializeField] private GameObject buyerSlotPrefab;
    [SerializeField] private TMP_Text buyerNameText;
    [SerializeField] private Image buyerIcon;
    [SerializeField] private Button completeTradeButton;

    private BuyerDataSO currentBuyer;
    private List<BuyerSlotUI> buyerSlots = new();

    private void Awake()
    {
        Instance = this;
        marketPanel.SetActive(false);
    }

    public void OpenMarket(BuyerDataSO buyer)
    {
        currentBuyer = buyer;
        marketPanel.SetActive(true);
        PopulateBuyerSlots();

        buyerNameText.text = buyer.buyerName;
        buyerIcon.sprite = buyer.buyerIcon;
        UIManager.Instance.ShowMessage($"🛒 Trading with {buyer.buyerName}");
    }

    private void PopulateBuyerSlots()
    {
        foreach (Transform child in buyerSlotParent)
            Destroy(child.gameObject);

        buyerSlots.Clear();

        foreach (var req in currentBuyer.requestedItems)
        {
            var slot = Instantiate(buyerSlotPrefab, buyerSlotParent).GetComponent<BuyerSlotUI>();
            slot.Setup(req);
            buyerSlots.Add(slot);
        }

        completeTradeButton.onClick.RemoveAllListeners();
        completeTradeButton.onClick.AddListener(TryCompleteTrade);
    }

    public void TryCompleteTrade()
    {
        if (AllSlotsFilled())
        {
            MarketTransactionManager.Instance.ProcessTrade(currentBuyer, buyerSlots);
            marketPanel.SetActive(false);
        }
        else
        {
            UIManager.Instance.ShowMessage("⚠️ Not all requested items delivered!");
        }
    }

    private bool AllSlotsFilled()
    {
        foreach (var slot in buyerSlots)
            if (!slot.IsFilled()) return false;

        return true;
    }

    public void CloseMarket()
    {
        marketPanel.SetActive(false);
    }
}
