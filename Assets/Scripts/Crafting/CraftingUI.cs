using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class CraftingUI : MonoBehaviour
{
    [Header("Panels & Prefabs")]
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private Transform ingredientParent;
    [SerializeField] private GameObject ingredientBoxPrefab;
    [SerializeField] private GameObject craftingUIMain;

    [Header("Detail References")]
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private Button craftButton;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private GameObject craftingTableRef;
    [SerializeField] private TMP_InputField quantityInput;
    private List<CraftingRecipeButton> recipeButtonList = new List<CraftingRecipeButton>();
    private List<CraftingRecipeButton> recipeButtons = new List<CraftingRecipeButton>();
    private List<CraftingIngredientBox> ingredientBoxes = new List<CraftingIngredientBox>();

    private CraftingRecipeSO currentRecipe;
    private int craftQuantity = 1;
    private void OnEnable()
    {
        RefreshRecipeList();
        ClearDetails();

        CraftingManager.Instance.onCraftingProgress += UpdateProgress;
        CraftingManager.Instance.onCraftingComplete += OnCraftingComplete;
        InventoryManager.onInventoryChanged += RefreshUI;


        progressBar.gameObject.SetActive(false);
        progressText.text = "";
    }

    private void OnDisable()
    {
        CraftingManager.Instance.onCraftingProgress -= UpdateProgress;
        CraftingManager.Instance.onCraftingComplete -= OnCraftingComplete;
        InventoryManager.onInventoryChanged -= RefreshUI;
    }

    private void RefreshRecipeList()
    {
        foreach (Transform child in recipeListParent)
            Destroy(child.gameObject);

        recipeButtons.Clear();

        foreach (var recipe in CraftingManager.Instance.GetAllRecipes())
        {
            var btnObj = Instantiate(recipeButtonPrefab, recipeListParent);
            var btn = btnObj.GetComponent<CraftingRecipeButton>();
            btn.Setup(recipe, this);
            recipeButtons.Add(btn);
        }
    }
    public void OpenCraftingTable()
    {
        craftingUIMain.SetActive(true);
    }
    public void OnRecipeSelected(CraftingRecipeSO recipe)
    {
        currentRecipe = recipe;
        ShowRecipeDetails(recipe);
    }
    public void HighlightButton(CraftingRecipeButton selected)
    {
        foreach (var btn in recipeButtons)
            btn.SetSelected(btn == selected);
    }
    private void RefreshUI()
    {
        if (currentRecipe == null) return;

        foreach (var box in ingredientBoxes)
            box.Refresh();

        UpdateCraftButtonState();
    }


    private void ShowRecipeDetails(CraftingRecipeSO recipe)
    {
        currentRecipe = recipe;
        recipeNameText.text = recipe.recipeName;

        foreach (Transform child in ingredientParent)
            Destroy(child.gameObject);

        ingredientBoxes.Clear(); // ✅ Clear list

        foreach (var ingredient in recipe.ingredients)
        {
            var boxObj = Instantiate(ingredientBoxPrefab, ingredientParent);
            var box = boxObj.GetComponent<CraftingIngredientBox>();
            box.Setup(ingredient);
            ingredientBoxes.Add(box); // ✅ Add to list
        }

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => TryCraft());

        UpdateCraftButtonState(); // ✅ Make button interactive/non-interactive
    }



    private void TryCraft()
    {
        if (currentRecipe == null) return;
        if (CraftingManager.Instance.IsCrafting()) return;

        if (!int.TryParse(quantityInput.text, out craftQuantity))
            craftQuantity = 1;

        craftQuantity = Mathf.Max(1, craftQuantity);

        progressBar.gameObject.SetActive(true);
        progressText.text = "0%";
        craftButton.interactable = false;

        CraftingManager.Instance.StartCrafting(currentRecipe, craftingTableRef, craftQuantity);
    }

    private void UpdateProgress(float progress)
    {
        progressBar.value = progress;
        progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    private void OnCraftingComplete()
    {
        progressBar.gameObject.SetActive(false);
        progressText.text = "";
        craftButton.interactable = true;

        ShowRecipeDetails(currentRecipe); // refresh ingredient counts
    }

    private void ClearDetails()
    {
        recipeNameText.text = "";
        foreach (Transform child in ingredientParent)
            Destroy(child.gameObject);
    }
    private void UpdateCraftButtonState()
    {
        if (currentRecipe == null)
        {
            craftButton.interactable = false;
            return;
        }

        int quantity = 1;

        if (quantityInput != null && int.TryParse(quantityInput.text, out int parsed))
            quantity = Mathf.Max(1, parsed);

        bool canCraft = CraftingManager.Instance.CanCraft(currentRecipe, quantity);
        craftButton.interactable = canCraft;
        craftButton.image.color = canCraft ? Color.white : new Color(1f, 1f, 1f, 0.5f);
    }


    private int GetCurrentQuantity()
    {
        if (!int.TryParse(quantityInput.text, out craftQuantity))
        {
            return craftQuantity;
        }
        return 0;
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void SetCraftingTable(GameObject table)
    {
        craftingTableRef = table;
    }
}
