// DeliveryMissionHandler.cs
using System.Linq;
using UnityEngine;

public class DeliveryMissionHandler : MissionHandler
{
    public override void OnMissionStart()
    {
        // Subscribe to delivery events
        MissionEvents.OnItemDelivered += HandleItemDelivered;
        // Initial UI update
        MissionManager.Instance?.NotifyMissionUpdated(mission);
    }

    public override void OnMissionEvent(ObjectiveType type, ItemDataSO item, int amount)
    {
        if (mission == null) return;
        if (type != ObjectiveType.DeliverItem) return;

        // Apply amount to matching objectives
        int remainingToProcess = amount;
        foreach (var obj in mission.objectives)
        {
            if (obj.type != ObjectiveType.DeliverItem) continue;
            if (obj.targetItem == null) continue;
            if (obj.targetItem != item) continue;
            if (obj.IsComplete) continue;

            int applied = obj.AddProgress(remainingToProcess);
            remainingToProcess -= applied;

            // update UI
            MissionManager.Instance?.NotifyMissionUpdated(mission);

            if (remainingToProcess <= 0) break;
        }

        // check completion
        if (CheckComplete())
        {
            Complete();
        }
    }

    private void HandleItemDelivered(ItemDataSO item, int amount)
    {
        OnMissionEvent(ObjectiveType.DeliverItem, item, amount);
    }

    private void Complete()
    {
        // award & cleanup
        MissionManager.Instance?.CompleteActiveMission(mission);
    }

    public override void OnMissionComplete()
    {
        MissionEvents.OnItemDelivered -= HandleItemDelivered;
    }
}
