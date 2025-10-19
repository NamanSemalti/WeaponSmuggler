using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BuyerData", menuName = "WeaponSmuggler/Buyer Data", order = 1)]
public class BuyerDataSO : ScriptableObject
{
    [Header("Identity")]
    public string buyerName;
    public Sprite buyerIcon;
    [TextArea] public string description;

    [Header("Economy / Demand")]
    [Tooltip("Base multiplier applied to item base value (1 = neutral).")]
    public float baseDemandMultiplier = 1f;      // used earlier as baseDemandMultiplier
    [Tooltip("Alternative name used elsewhere. Kept for safety.")]
    [FormerlySerializedAs("demandMultiplier")]
    public float demandMultiplier = 1f;         // kept for compatibility

    [Header("Risk & Reputation")]
    [Tooltip("How much heat (risk) this buyer adds per sale.")]
    public float riskMultiplier = 1f;
    [Tooltip("Minimum reputation required to access this buyer.")]
    public float reputationRequirement = 0f;

    [Header("Legality")]
    [Tooltip("True if buyer is a legal buyer; false if black market (illegal).")]
    public bool isLegal = false;
    // [FormerlySerializedAs("isIllegal")]
    public bool isIllegal // legacy field name compatibility
    {
        // auto-sync for older serialized assets that used isIllegal
        get => !isLegal;
        set => isLegal = !value;
    }

    [Header("Requested Items (Buyer Inventory / Requests)")]
    public List<BuyerRequest> requestedItems = new List<BuyerRequest>();
}

[System.Serializable]
public class BuyerRequest
{
    public ItemDataSO item;
    public int requiredQuantity = 1;
    public float offeredPricePerUnit = 0f; // optional override price
}
