using UnityEngine;
using TMPro;

public class VehicleCargoInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask vehicleSlotLayer;
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private TMP_Text quantityUI; // UI showing how much player will unload
    [SerializeField] private Transform dropSpawnPoint; // spawn overflow items here

    private VehicleSlotUI currentTargetSlot;
    private bool isHoldingE;
    private int selectedQuantity = 1;
    private float lastScrollTime;

    private void Start()
    {
        if (quantityUI != null)
            quantityUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        DetectSlot();

        if (currentTargetSlot == null) return;

        // Handle input manually
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.IsScrollBusy = true;
            Debug.Log("<color=cyan>[INPUT]</color> Started holding E");
            isHoldingE = true;
            selectedQuantity = 1;

            if (quantityUI != null)
            {
                quantityUI.gameObject.SetActive(true);
                quantityUI.text = $"Unload: {selectedQuantity}";
            }
        }

        if (isHoldingE)
        {

            HandleScrollSelection();
        }

        if (Input.GetKeyUp(KeyCode.E) && isHoldingE)
        {
            Debug.Log("<color=green>[INPUT]</color> Released E - Triggering unload");
            HandleUnload();
            isHoldingE = false;

            if (quantityUI != null)
                quantityUI.gameObject.SetActive(false);
            UIManager.Instance.HideInteractionMessage();
            GameManager.Instance.IsScrollBusy = false;
        }
    }

    private void DetectSlot()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, vehicleSlotLayer))
        {
            var slotUI = hit.collider.GetComponent<VehicleSlotUI>();
            if (slotUI != null)
            {
                if (slotUI != currentTargetSlot)
                {
                    ClearHighlight();
                    currentTargetSlot = slotUI;
                    slotUI.SetHighlighted(true);
                    Debug.Log($"<color=orange>[RAYCAST]</color> Highlighting slot: {slotUI.name}");
                }
                return;
            }
        }

        ClearHighlight();
    }

    private void ClearHighlight()
    {
        if (currentTargetSlot != null)
        {
            currentTargetSlot.SetHighlighted(false);
            Debug.Log("<color=gray>[HIGHLIGHT]</color> Cleared highlight");
            currentTargetSlot = null;
        }
    }

    private void HandleScrollSelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f && Time.time - lastScrollTime > 0.02f)
        {
            int dir = scroll > 0 ? 1 : -1;
            int maxQty = currentTargetSlot != null ? currentTargetSlot.GetQuantity() : 1;

            selectedQuantity = Mathf.Clamp(selectedQuantity + dir, 1, maxQty);
            lastScrollTime = Time.time;

            Debug.Log($"<color=magenta>[SCROLL]</color> Adjusted Qty: {selectedQuantity}/{maxQty}");

            if (quantityUI != null)
                quantityUI.text = $"Unload: {selectedQuantity}/{maxQty}";
            if (selectedQuantity == 0) return;
            if (selectedQuantity == 1)
                UIManager.Instance.ShowInteractionMessage(currentTargetSlot.GetItem()?.itemName);
            else
                UIManager.Instance.ShowInteractionMessage(currentTargetSlot.GetItem()?.itemName + " " + "x" + selectedQuantity);
        }
    }

    private void HandleUnload()
    {
        if (currentTargetSlot == null)
        {
            Debug.LogWarning("<color=red>[UNLOAD]</color> No target slot found!");
            return;
        }

        var item = currentTargetSlot.GetItem();
        if (item == null)
        {
            Debug.LogWarning("<color=red>[UNLOAD]</color> Slot has no item!");
            return;
        }

        int qty = selectedQuantity;
        Debug.Log($"<color=green>[UNLOAD]</color> Trying to unload {qty}x {item.itemName}");

        var cargo = currentTargetSlot.LinkedCargo;
        if (cargo == null)
        {
            Debug.LogError("<color=red>[UNLOAD]</color> LinkedCargo is NULL!");
            return;
        }

        int added = 0;

        if (InventoryManager.Instance != null)
        {
            for (int i = 0; i < qty; i++)
            {
                bool success = InventoryManager.Instance.AddItem(item, 1);
                if (!success)
                {
                    Debug.Log("<color=yellow>[UNLOAD]</color> Inventory full — cannot add more!");
                    break;
                }
                added++;
            }
        }
        else
        {
            Debug.LogError("<color=red>[UNLOAD]</color> InventoryManager.Instance is NULL!");
        }

        int leftover = qty - added;
        if (leftover > 0)
        {
            Debug.Log($"<color=orange>[UNLOAD]</color> Spawning {leftover}x leftover items near vehicle");

            for (int i = 0; i < leftover; i++)
            {
                var obj = ObjectPoolManager.Instance.GetFromPool(item.handPoolKey);
                if (obj != null)
                {
                    obj.transform.position = dropSpawnPoint.position;
                    obj.transform.rotation = dropSpawnPoint.rotation;

                    if (obj.TryGetComponent(out Rigidbody rb))
                        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                }
            }
        }

        // Remove items from cargo
        int removed = cargo.RemoveMatchingItems(item, qty);
        Debug.Log($"<color=green>[UNLOAD]</color> Removed {removed}x {item.itemName} from cargo");

        UIManager.Instance?.ShowMessage($"📦 Unloaded {qty}x {item.itemName}");

        selectedQuantity = 1;
        cargo.OnInventoryChanged?.Invoke();
    }
}
