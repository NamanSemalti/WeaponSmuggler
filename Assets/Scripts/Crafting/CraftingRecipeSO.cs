using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "WeaponSmuggler/Crafting Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [Header("Recipe Info")]
    public string recipeID;
    public string recipeName;
    public CraftingCategory category;
    public Sprite recipeIcon; // ✅ NEW — used for UI buttons
    [Header("Ingredients Required")]
    public List<Ingredient> ingredients = new List<Ingredient>();

    [Header("Resulting Items")]
    public List<CraftedResult> results = new List<CraftedResult>();

    [Header("Crafting Properties")]
    public float craftTime = 0f; // 0 = instant
    public int requiredSkillLevel = 0;
    public int heatModifier = 0; // risk level change

    [System.Serializable]
    public class Ingredient
    {
        public ItemDataSO item;
        public int quantity;
    }

    [System.Serializable]
    public class CraftedResult
    {
        public ItemDataSO item;
        public int quantity;
    }
}

public enum CraftingCategory
{
    Weapon,
    Part,
    Concealment,
    Vehicle,
    Document,
    Misc
}
