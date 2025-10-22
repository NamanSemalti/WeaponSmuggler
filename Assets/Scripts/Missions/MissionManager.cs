// MissionManager.cs
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Mission Database (assign assets)")]
    [SerializeField] private List<MissionDataSO> missionDatabase = new List<MissionDataSO>();

    private MissionDataSO activeMission;
    private MissionHandler activeHandler;

    public delegate void MissionUpdateDelegate(MissionDataSO mission);
    public event MissionUpdateDelegate OnMissionUpdated;
    public event MissionUpdateDelegate OnMissionCompleted;
    public event MissionUpdateDelegate OnMissionStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        StartMission("mission1");
    }
    public bool StartMission(string missionID)
    {
        var mission = missionDatabase.Find(m => m.missionID == missionID);
        if (mission == null)
        {
            Debug.LogWarning($"MissionManager: mission '{missionID}' not found in database.");
            return false;
        }

        if (activeHandler != null)
        {
            Debug.LogWarning("MissionManager: another mission is already active.");
            return false;
        }

        // Basic requirement check
        if (mission.requiredReputation > 0)
        {
            if (HeatReputationManager.Instance.GetReputationValue() < mission.requiredReputation)
            {
                UIManager.Instance?.ShowMessage("Reputation too low for this mission.");
                return false;
            }
        }

        activeMission = mission;
        activeHandler = CreateHandlerForMission(mission);
        activeHandler.Initialize(mission);

        OnMissionStarted?.Invoke(mission);
        UIManager.Instance?.ShowMessage($"📜 Mission started: {mission.missionName}");
        NotifyMissionUpdated(mission);
        return true;
    }

    private MissionHandler CreateHandlerForMission(MissionDataSO mission)
    {
        switch (mission.missionType)
        {
            case MissionType.Delivery:
                return new DeliveryMissionHandler();
            // add other mission types here when ready
            default:
                return new DeliveryMissionHandler(); // fallback
        }
    }

    public void NotifyMissionUpdated(MissionDataSO mission)
    {
        OnMissionUpdated?.Invoke(mission);
        MissionUI.Instance?.UpdateMissionUI(mission);
    }

    public void CompleteActiveMission(MissionDataSO mission)
    {
        if (activeMission == null || mission != activeMission) return;

        // Reward the player
        if (mission.cashReward > 0) PlayerEconomy.Instance?.AddMoney(mission.cashReward);
        if (mission.reputationReward != 0) HeatReputationManager.Instance?.AddReputation(mission.reputationReward);
        if (mission.itemReward != null) InventoryManager.Instance?.AddItem(mission.itemReward);

        UIManager.Instance?.ShowMessage($"✅ Mission complete: {mission.missionName}");
        OnMissionCompleted?.Invoke(mission);

        // End & cleanup
        activeHandler?.EndMission();
        activeHandler = null;
        activeMission = null;
    }

    public MissionDataSO GetActiveMission() => activeMission;

    private void OnDestroy()
    {
        if (activeHandler != null)
        {
            activeHandler.EndMission();
            activeHandler = null;
        }
    }
}
