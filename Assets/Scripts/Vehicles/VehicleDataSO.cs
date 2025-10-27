using UnityEngine;

[CreateAssetMenu(menuName = "WeaponSmuggler/Vehicle Data", fileName = "VehicleDataSO")]
public class VehicleDataSO : ScriptableObject
{
    [Header("Identity")]
    public string vehicleName;
    public Sprite vehicleIcon;
    public GameObject vehiclePrefab;

    [Header("Performance")]
    public float maxSpeed = 50f;
    public float acceleration = 10f;
    public float braking = 8f;

    [Header("Cargo")]
    [Tooltip("Number of item slots in this vehicle")]
    public int cargoSlots = 10;
    [Tooltip("Maximum total weight the cargo can hold")]
    public float cargoCapacity = 200f;

    [Header("Detection / Risk")]
    [Range(0f, 1f)]
    public float detectionRisk = 0.15f;
    public float heatMultiplier = 1.0f;
}
