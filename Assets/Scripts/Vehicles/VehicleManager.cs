using UnityEngine;

/// <summary>
/// Responsible for spawning and tracking the active player vehicle instance.
/// Keeps a reference to the active VehicleCargo for UI & mission integration.
/// </summary>
public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance { get; private set; }

    [SerializeField] private Transform vehicleSpawnPoint;
    [SerializeField] private bool persistBetweenScenes = false;

    private GameObject spawnedVehicle;
    private VehicleCargo activeCargo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (persistBetweenScenes) DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Spawns the vehicle prefab described by vehicleData at the spawn point.
    /// If a vehicle is already present it will be destroyed first.
    /// </summary>
    public void SpawnVehicle(VehicleDataSO vehicleData)
    {
        if (vehicleData == null)
        {
            Debug.LogError("VehicleManager.SpawnVehicle called with null data.");
            return;
        }

        DespawnVehicle();

        if (vehicleData.vehiclePrefab == null)
        {
            Debug.LogError($"Vehicle prefab not set for {vehicleData.vehicleName}");
            return;
        }

        Vector3 pos = vehicleSpawnPoint != null ? vehicleSpawnPoint.position : Vector3.zero;
        Quaternion rot = vehicleSpawnPoint != null ? vehicleSpawnPoint.rotation : Quaternion.identity;

        spawnedVehicle = Instantiate(vehicleData.vehiclePrefab, pos, rot);
        if (spawnedVehicle == null)
        {
            Debug.LogError("Vehicle spawn failed.");
            return;
        }

        activeCargo = spawnedVehicle.GetComponent<VehicleCargo>();
        if (activeCargo == null)
        {
            // Add component if missing (safer), but warn designer
            Debug.LogWarning("Spawned vehicle does not have VehicleCargo; adding one dynamically.");
            activeCargo = spawnedVehicle.AddComponent<VehicleCargo>();
            // attempt to assign the data so cargo has vehicleData
            var cargoField = activeCargo.GetType().GetField("vehicleData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (cargoField != null) cargoField.SetValue(activeCargo, vehicleData);
        }

        UIManager.Instance?.ShowMessage($"🚚 Spawned {vehicleData.vehicleName}");
    }

    public void DespawnVehicle()
    {
        if (spawnedVehicle != null)
        {
            Destroy(spawnedVehicle);
            spawnedVehicle = null;
            activeCargo = null;
        }
    }
    public void SetActiveVehicle(VehicleCargo cargo)
    {
        activeCargo = cargo;
    }
    public VehicleCargo GetActiveCargo() => activeCargo;
    public GameObject GetActiveVehicleGameObject() => spawnedVehicle;
}
