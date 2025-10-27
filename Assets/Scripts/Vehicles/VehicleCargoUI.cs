using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class VehicleCargoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VehicleCargo targetCargo;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TMP_Text capacityText;

    private List<VehicleSlotUI> slots = new();

    private void Start()
    {
        if (targetCargo != null)
        {
            targetCargo.OnCargoUpdated.AddListener(RefreshUI);
            BuildSlots();
            RefreshUI();
        }
    }

    private void OnDestroy()
    {
        if (targetCargo != null)
            targetCargo.OnCargoUpdated.RemoveListener(RefreshUI);
    }

    private void BuildSlots()
    {
        ClearSlots();

        for (int i = 0; i < targetCargo.CargoSlots; i++)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var ui = go.GetComponent<VehicleSlotUI>();
            ui.Bind(targetCargo);
            slots.Add(ui);
        }
    }

    private void ClearSlots()
    {
        foreach (var s in slots)
        {
            if (s != null) Destroy(s.gameObject);
        }
        slots.Clear();
    }

    public void RefreshUI()
    {
        var cargoSlots = targetCargo.GetCargoSlots();

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < cargoSlots.Count)
                slots[i].SetupSlot(cargoSlots[i].itemData, cargoSlots[i].quantity);
            else
                slots[i].SetupSlot(null, 0);
        }

        if (capacityText)
            capacityText.text = $"{Mathf.RoundToInt(targetCargo.GetWeightPercent() * 100)}% Full";
    }
}
