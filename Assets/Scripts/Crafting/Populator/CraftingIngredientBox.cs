using UnityEngine;

public class CraftingIngredientBox : CraftingUIElementBase
{
    private CraftingRecipeSO.Ingredient ingredientData;

    public void Setup(CraftingRecipeSO.Ingredient ingredient)
    {
        ingredientData = ingredient;

        if (ingredient.item == null)
        {
            SetLabel("N/A", lockedColor);
            SetIcon(null);
            return;
        }

        SetIcon(ingredient.item.icon);

        int owned = InventoryManager.Instance.GetItemQuantity(ingredient.item);
        int required = ingredient.quantity;
        bool hasEnough = owned >= required;

        SetLabel($"{owned} / {required}", hasEnough ? normalColor : insufficientColor);
    }

    public override void Refresh()
    {
        if (ingredientData == null || ingredientData.item == null) return;

        int owned = InventoryManager.Instance.GetItemQuantity(ingredientData.item);
        int required = ingredientData.quantity;
        bool hasEnough = owned >= required;

        SetLabel($"{owned} / {required}", hasEnough ? normalColor : insufficientColor);
    }
}
