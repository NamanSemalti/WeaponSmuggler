using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyerItemPopulator : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;
    private int requiredQuantity;
    private int currentQuantity;
    private string requirementID;
    public void PopulateBuyerItem(Requirement requirement)
    {
        if (requirement.item.icon) itemImage.sprite = requirement.item.icon;
        requiredQuantity = requirement.requiredQuantity;
        itemCountText.text = currentQuantity + "/" + requiredQuantity;
        requirementID = requirement.requirementID;
    }
    public bool TryIncreaseQuantity(string _requirementID, int amountToIncrease)
    {
        if (IsRequirementIdMatching(_requirementID))
        {
            currentQuantity = Mathf.Min(currentQuantity + amountToIncrease, requiredQuantity);
            itemCountText.text = currentQuantity + "/" + requiredQuantity;
            return true;
        }
        else return false;
    }
    private bool IsRequirementIdMatching(string _requirementID)
    {
        return requirementID == _requirementID;
    }
}
