using UnityEngine;

/// <summary>
/// Interactable component placed directly on a vehicle.
/// When the player interacts, it opens that vehicle's cargo UI.
/// </summary>
[RequireComponent(typeof(VehicleCargo))]
public class InteractableVehicle : Interactable
{
    private VehicleCargo vehicleCargo;

    protected override void Awake()
    {
        base.Awake();
        vehicleCargo = GetComponent<VehicleCargo>();
    }

    protected override void OnInteract()
    {
        var cargo = GetComponent<VehicleCargo>();
        if (cargo == null)
        {
            Debug.LogWarning("No VehicleCargo found on this vehicle!");
            return;
        }

        VehicleManager.Instance.SetActiveVehicle(cargo);
        VehicleUI.Instance.Open(cargo);
    }

}
