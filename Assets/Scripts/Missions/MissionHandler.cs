// MissionHandler.cs
using UnityEngine;

public abstract class MissionHandler
{
    protected MissionDataSO mission;
    protected bool isActive = false;

    public virtual void Initialize(MissionDataSO missionData)
    {
        mission = missionData;
        isActive = true;
        ResetObjectivesProgress();
        OnMissionStart();
    }

    protected virtual void ResetObjectivesProgress()
    {
        if (mission == null) return;
        foreach (var obj in mission.objectives)
            obj.ResetProgress();
    }

    public abstract void OnMissionStart();
    public abstract void OnMissionEvent(ObjectiveType type, ItemDataSO item, int amount);
    public abstract void OnMissionComplete();

    public virtual bool CheckComplete()
    {
        if (mission == null) return false;
        foreach (var obj in mission.objectives)
            if (!obj.IsComplete) return false;
        return true;
    }

    public virtual void EndMission()
    {
        isActive = false;
        OnMissionComplete();
    }
}
