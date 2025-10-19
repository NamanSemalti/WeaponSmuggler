using UnityEngine;

public class HeatReputationManager : MonoBehaviour
{
    public static HeatReputationManager Instance { get; private set; }

    [SerializeField] private float heat;
    [SerializeField] private float reputation;

    private const float MaxHeat = 100f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddHeat(float value)
    {
        heat = Mathf.Clamp(heat + value, 0, MaxHeat);
    }

    public void AddReputation(float value)
    {
        reputation += value;
    }

    public float GetHeatPenalty()
    {
        return Mathf.Clamp01(heat / MaxHeat * 0.5f); // up to 50% penalty at max heat
    }

    public float GetHeat() => heat;
    public float GetReputation() => reputation;
}
