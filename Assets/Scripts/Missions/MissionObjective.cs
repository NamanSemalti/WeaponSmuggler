// MissionObjective.cs
using System;
using UnityEngine;

[Serializable]
public enum ObjectiveType
{
    DeliverItem,
    CraftItem,
    LoadCargo,
    VisitLocation,
    Timed
}

[Serializable]
public class MissionObjective
{
    public string objectiveID;
    public string objectiveName;
    [TextArea] public string description;

    public ObjectiveType type;
    public ItemDataSO targetItem;    // the item to deliver/craft/load (if applicable)
    public int requiredAmount = 1;

    [NonSerialized] public int currentAmount = 0;

    public bool IsComplete => currentAmount >= requiredAmount;

    public void ResetProgress() => currentAmount = 0;

    public int AddProgress(int amount)
    {
        if (IsComplete) return 0;
        int remaining = Mathf.Max(0, requiredAmount - currentAmount);
        int toAdd = Mathf.Min(remaining, amount);
        currentAmount += toAdd;
        return toAdd;
    }
}
