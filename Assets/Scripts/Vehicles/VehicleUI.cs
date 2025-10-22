using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fullscreen vehicle UI. Open with VehicleUI.Instance.Open(cargo)
/// </summary>
public class VehicleUI : MonoBehaviour
{
    public static VehicleUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text vehicleNameText;
    [SerializeField] private TMP_Text capacityText;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Image vehicleIcon;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private RectTransform buyerSlotContentSizeFitter;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Button closeButton;

    private VehicleCargo boundCargo;
    private List<VehicleSlotUI> slotUIs = new List<VehicleSlotUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (panelRoot != null) panelRoot.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    public void Open(VehicleCargo cargo)
    {
        if (cargo == null)
        {
            Debug.LogWarning("VehicleUI.Open called with null cargo.");
            return;
        }

        boundCargo = cargo;
        BindUI();
        StartCoroutine(RebuildNextFrame());
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Close()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (boundCargo != null)
        {
            boundCargo.OnCargoChanged -= RefreshUI;
            boundCargo = null;
        }
    }
    private IEnumerator RebuildNextFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(buyerSlotContentSizeFitter);
    }
    private void BindUI()
    {
        // Unsubscribe previous
        if (boundCargo != null) boundCargo.OnCargoChanged -= RefreshUI;

        // clear existing slots
        foreach (Transform t in slotContainer) Destroy(t.gameObject);
        slotUIs.Clear();

        // header
        if (vehicleNameText != null && boundCargo.VehicleData != null)
            vehicleNameText.text = boundCargo.VehicleData.vehicleName;
        if (vehicleIcon != null) vehicleIcon.sprite = boundCargo.VehicleData?.vehicleIcon;
        RefreshUI();

        // instantiate slot UIs
        var slots = boundCargo.GetCargoSlots();
        int expected = boundCargo.GetMaxSlots();
        // we want to visually show all possible slots (including empty)
        for (int i = 0; i < expected; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            var slotUI = go.GetComponent<VehicleSlotUI>();
            // We bind the cargo and index – slot may be empty but UI shows it
            slotUI.Bind(boundCargo, i);
            slotUIs.Add(slotUI);
        }
        boundCargo.OnCargoChanged += RefreshUI;
    }

    public void RefreshUI()
    {
        if (boundCargo == null) return;

        float currentWeight = boundCargo.GetCurrentCargoWeight();
        float capacity = boundCargo.GetMaxCapacity();

        if (weightText != null) weightText.text = $"Weight: {currentWeight:0.0}/{capacity:0.0}";
        if (capacityText != null) capacityText.text = $"Slots: {boundCargo.GetCargoSlots().Count}/{boundCargo.GetMaxSlots()}";

        // Show performance stats
        if (statsText != null && boundCargo.VehicleData != null)
        {
            var d = boundCargo.VehicleData;
            statsText.text = $"Speed: {d.maxSpeed:0}  Accel: {d.acceleration:0}  Risk: {d.detectionRisk:0.00}";
        }

        // Refresh each slot UI (they subscribe to OnCargoChanged and will refresh themselves)
        foreach (var su in slotUIs) su.Refresh();
    }
}
