using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemInteractable : Interactable
{
    [Header("Shop Config")]
    [SerializeField] private ShopItemSO shopItem;

    [Header("UI Elements")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private TMP_Text descTxt;
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text qtyTxt;

    private bool holdingE = false;
    private int selectedQty = 1;
    private float lastScrollTime;

    protected override void Awake()
    {
        base.Awake();
        if (infoPanel) infoPanel.SetActive(false);
        if (qtyTxt) qtyTxt.gameObject.SetActive(false);
    }

    protected override void OnFocus()
    {
        ShowInfo();
    }

    protected override void OnLoseFocus()
    {
        if (infoPanel) infoPanel.SetActive(false);
        if (qtyTxt) qtyTxt.gameObject.SetActive(false);
        holdingE = false;
    }

    protected override void OnInteract()
    {
        // Start hold mode
        if (!holdingE)
        {
            holdingE = true;
            selectedQty = 1;
            qtyTxt?.gameObject.SetActive(true);
            UpdateQtyUI();
            StartCoroutine(QuantitySelectionRoutine());
        }
    }

    private System.Collections.IEnumerator QuantitySelectionRoutine()
    {
        while (holdingE && IsFocused)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                // released E → buy
                holdingE = false;
                qtyTxt?.gameObject.SetActive(false);
                TryBuy(selectedQty);
                yield break;
            }

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.1f && Time.time - lastScrollTime > 0.12f)
            {
                int dir = scroll > 0 ? 1 : -1;
                selectedQty = Mathf.Clamp(selectedQty + dir, 1, 999);
                lastScrollTime = Time.time;
                UpdateQtyUI();
            }

            yield return null;
        }
    }

    private void ShowInfo()
    {
        if (!shopItem || !shopItem.item) return;

        infoPanel?.SetActive(true);
        nameTxt.text = shopItem.item.itemName;
        priceTxt.text = $"Price: ${shopItem.pricePerUnit}";
        descTxt.text = shopItem.description;
        iconImg.sprite = shopItem.item.icon;
    }

    void UpdateQtyUI()
    {
        if (qtyTxt)
            qtyTxt.text = $"Buy: {selectedQty}";
    }

    private void TryBuy(int qty)
    {
        int totalCost = qty * shopItem.pricePerUnit;

        if (!PlayerEconomy.Instance.HasMoney(totalCost))
        {
            UIManager.Instance?.ShowMessage("❌ Not enough money!");
            return;
        }

        int added = 0;
        for (int i = 0; i < qty; i++)
        {
            if (InventoryManager.Instance.AddItem(shopItem.item, 1))
                added++;
            else break;
        }

        // Deduct money only for items successfully stored
        PlayerEconomy.Instance.SpendMoney(added * shopItem.pricePerUnit);

        UIManager.Instance?.ShowMessage(
            $"✅ Bought {added}x {shopItem.item.itemName}");

        int leftover = qty - added;
        if (leftover > 0)
        {
            SpawnLeftover(leftover);
        }
    }

    private void SpawnLeftover(int count)
    {
        var obj = ObjectPoolManager.Instance.GetFromPool(shopItem.item.handPoolKey);
        obj.transform.position = transform.position + transform.forward * 1f;

        if (obj.TryGetComponent(out PickableItem p))
            p.SetItemCount(count);
    }
}
