using UnityEngine;
using TMPro;

public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }
    [SerializeField] private float money;
    [SerializeField] private TMP_Text moneyText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddMoney(float amount)
    {
        money += amount;
        UpdateUI();
    }

    public bool SpendMoney(float amount)
    {
        if (money < amount) return false;
        money -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (moneyText)
            moneyText.text = $"${money:N0}";
    }
}
