using UnityEngine;
using System;

public class HeatReputationManager : MonoBehaviour
{
    public static HeatReputationManager Instance { get; private set; }

    [Header("Heat Settings")]
    [SerializeField] private float heat;
    [SerializeField] private const float MaxHeat = 100f;
    [SerializeField] private float heatDecayRate = 1f; // per minute or per mission
    public event Action<float> OnHeatChanged;

    [Header("Reputation Settings")]
    [SerializeField] private float reputation;
    [SerializeField] private float minReputation = -100f;
    [SerializeField] private float maxReputation = 100f;
    public event Action<float> OnReputationChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Optional: passive heat decay
        if (heat > 0)
        {
            heat = Mathf.Max(0, heat - heatDecayRate * Time.deltaTime);
        }
    }

    // ---------------------------
    // 🔥 HEAT SYSTEM
    // ---------------------------

    public void AddHeat(float value)
    {
        heat = Mathf.Clamp(heat + value, 0, MaxHeat);
        OnHeatChanged?.Invoke(heat);
        UIManager.Instance?.ShowMessage($"🔥 Heat level: {Mathf.RoundToInt(heat)}%");
    }

    public float GetHeat() => heat;

    public float GetHeatPenalty()
    {
        // Used in market or mission difficulty
        return Mathf.Clamp01(heat / MaxHeat * 0.5f); // up to 50% penalty at max heat
    }

    public void ResetHeat()
    {
        heat = 0;
        OnHeatChanged?.Invoke(heat);
    }

    // ---------------------------
    // ⭐ REPUTATION SYSTEM
    // ---------------------------

    public void AddReputation(float value)
    {
        float oldValue = reputation;
        reputation = Mathf.Clamp(reputation + value, minReputation, maxReputation);

        if (reputation != oldValue)
        {
            OnReputationChanged?.Invoke(reputation);
            UIManager.Instance?.ShowMessage(
                value > 0 ? $"📈 Reputation +{value}" : $"📉 Reputation {value}");
        }
    }

    public float GetReputation() => reputation;

    // ✅ For compatibility with MissionManager
    public int GetReputationValue() => Mathf.RoundToInt(reputation);

    public float GetReputationNormalized()
    {
        return Mathf.InverseLerp(minReputation, maxReputation, reputation);
    }

    public void SetReputation(float value)
    {
        reputation = Mathf.Clamp(value, minReputation, maxReputation);
        OnReputationChanged?.Invoke(reputation);
    }

    public void ResetReputation()
    {
        SetReputation(0);
    }
}
