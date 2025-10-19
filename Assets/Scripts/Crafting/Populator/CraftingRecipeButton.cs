using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeButton : CraftingUIElementBase
{
    [SerializeField] private Button button;
    private CraftingRecipeSO recipeData;
    private CraftingUI parentUI;
    private bool isUnlocked;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(CraftingRecipeSO recipe, CraftingUI parent)
    {
        recipeData = recipe;
        parentUI = parent;
        isUnlocked = CraftingManager.Instance.IsRecipeUnlocked(recipe);

        SetIcon(recipe.recipeIcon ?? recipe.results[0].item.icon);
        Debug.Log("recipe name " + recipe.recipeName);
        SetLabel(isUnlocked ? recipe.recipeName : $"🔒 {recipe.recipeName}", isUnlocked ? normalColor : lockedColor);

        button.interactable = isUnlocked;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        if (!isUnlocked) return;
        parentUI.OnRecipeSelected(recipeData);
        parentUI.HighlightButton(this);
    }

    public override void Refresh()
    {
        // Future: Refresh recipe state (e.g., if recipe becomes unlocked mid-game)
        Setup(recipeData, parentUI);
    }
}
