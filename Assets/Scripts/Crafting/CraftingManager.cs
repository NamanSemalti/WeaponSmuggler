using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [SerializeField] private List<CraftingRecipeSO> allRecipes = new List<CraftingRecipeSO>();
    [SerializeField] private List<CraftingRecipeSO> unlockedRecipes = new List<CraftingRecipeSO>();

    public System.Action<float> onCraftingProgress; // ✅ event for progress
    public System.Action onCraftingComplete;

    private bool isCrafting = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public List<CraftingRecipeSO> GetAllRecipes() => allRecipes;
    public bool IsRecipeUnlocked(CraftingRecipeSO recipe) => unlockedRecipes.Contains(recipe);
    public bool IsCrafting() => isCrafting;

    public bool CanCraft(CraftingRecipeSO recipe)
    {
        foreach (var ing in recipe.ingredients)
        {
            if (InventoryManager.Instance.GetItemQuantity(ing.item) < ing.quantity)
                return false;
        }
        return true;
    }

    public void StartCrafting(CraftingRecipeSO recipe, GameObject craftingTable, int quantity = 1)
    {
        if (isCrafting)
        {
            UIManager.Instance?.ShowMessage("⚠️ Already crafting something!");
            return;
        }

        if (!CanCraft(recipe, quantity))
        {
            UIManager.Instance?.ShowMessage("❌ Not enough ingredients!");
            return;
        }

        // Remove ingredients
        foreach (var ing in recipe.ingredients)
            InventoryManager.Instance.RemoveItem(ing.item, ing.quantity * quantity);

        StartCoroutine(CraftingRoutine(recipe, craftingTable, quantity));
    }
    public bool CanCraft(CraftingRecipeSO recipe, int quantity = 1)
    {
        foreach (var ing in recipe.ingredients)
        {
            int owned = InventoryManager.Instance.GetItemQuantity(ing.item);
            if (owned < ing.quantity * quantity)
                return false;
        }
        return true;
    }



    private IEnumerator CraftingRoutine(CraftingRecipeSO recipe, GameObject table, int quantity)
    {
        isCrafting = true;
        float duration = Mathf.Max(recipe.craftTime * quantity, 0.1f);
        UIManager.Instance?.ShowMessage($"⚙️ Crafting {quantity}× {recipe.recipeName}...");

        for (int i = 0; i < quantity; i++)
        {
            float timer = 0f;
            while (timer < recipe.craftTime)
            {
                timer += Time.deltaTime;
                float overallProgress = (i + (timer / recipe.craftTime)) / quantity;
                onCraftingProgress?.Invoke(overallProgress);
                yield return null;
            }

            // After each item
            foreach (var result in recipe.results)
            {
                InventoryManager.Instance.AddItem(result.item, result.quantity);
                if (table != null && result.item.worldPrefab != null)
                {
                    Vector3 spawnPos = table.transform.position + table.transform.forward * 1.5f + Vector3.up * 0.5f + Random.insideUnitSphere * 0.2f;
                    Instantiate(result.item.worldPrefab, spawnPos, Quaternion.identity);
                }
            }
        }

        UIManager.Instance?.ShowMessage($"✅ Crafted {quantity}× {recipe.recipeName}!");
        isCrafting = false;
        onCraftingProgress?.Invoke(0f);
        onCraftingComplete?.Invoke();
    }

}
