// MissionDataSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "WeaponSmuggler/MissionData")]
public class MissionDataSO : ScriptableObject
{
    [Header("Identity")]
    public string missionID;
    public string missionName;
    [TextArea] public string missionDescription;
    public Sprite missionIcon;

    [Header("Type")]
    public MissionType missionType = MissionType.Delivery;

    [Header("Objectives")]
    public List<MissionObjective> objectives = new List<MissionObjective>();

    [Header("Rewards")]
    public int cashReward = 0;
    public int reputationReward = 0;
    public ItemDataSO itemReward = null;

    [Header("Constraints")]
    public int requiredReputation = 0;
    public string prerequisiteMissionID;

    private void OnEnable()
    {
        // When ScriptableObject reloads in Editor, clear runtime progress
        foreach (var obj in objectives)
            obj.currentAmount = 0;
    }
}

public enum MissionType
{
    Delivery,
    Crafting,
    SupplyRun,
    Escort,
    Special
}
