using UnityEngine;

public class BuyerInteractable : Interactable
{
    [SerializeField] private BuyerItemPopulator[] buyerItemPopulators;
    protected override void Awake()
    {
        base.Awake();
    }
    public void PopulateRequirementContainer(Requirement[] requirements)
    {
        int limit = Mathf.Min(requirements.Length, buyerItemPopulators.Length);
        for (int i = 0; i < limit; i++)
        {
            buyerItemPopulators[i].PopulateBuyerItem(requirements[i]);
            buyerItemPopulators[i].gameObject.SetActive(true);
        }
    }

    public void IncrementCurrentQuantity(string requirementId, int amountToIncrease)
    {
        for (int i = 0; i < buyerItemPopulators.Length; i++)
        {
            buyerItemPopulators[i].TryIncreaseQuantity(requirementId, amountToIncrease);
        }
    }
    protected override void OnFocus()
    {
        if (buyerItemPopulators.Length > 0)
            SetDisplayUI(true);
        base.OnFocus();
    }

    protected override void OnLoseFocus()
    {
        SetDisplayUI(false);
        base.OnLoseFocus();
    }
    protected override void OnInteract()
    {

    }
}
