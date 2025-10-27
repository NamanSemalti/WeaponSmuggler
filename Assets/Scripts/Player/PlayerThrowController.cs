using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerThrowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform throwOrigin;
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private float upwardModifier = 1.2f;
    [SerializeField] private GameObject throwUI;
    [SerializeField] private TMP_Text throwQtyText;

    private bool isHoldingThrow;
    private int selectedQty = 1;
    private float lastScrollTime;
    private Coroutine holdRoutine;

    private void Awake()
    {
        if (throwUI != null)
            throwUI.SetActive(false);
    }

    private void Update()
    {
        HandleThrowInput();
    }

    private void HandleThrowInput()
    {
        // Detect Q press (begin hold)
        if (Input.GetKeyDown(KeyCode.Q) && !isHoldingThrow)
        {
            isHoldingThrow = true;
            GameManager.Instance.IsScrollBusy = true;
            holdRoutine = StartCoroutine(HandleHoldLogic());
        }

        // Detect Q release
        if (Input.GetKeyUp(KeyCode.Q) && isHoldingThrow)
        {
            isHoldingThrow = false;
            GameManager.Instance.IsScrollBusy = false;
            if (holdRoutine != null)
            {
                StopCoroutine(holdRoutine);
                holdRoutine = null;
            }

            if (throwUI != null)
                throwUI.SetActive(false);

            // Perform throw on release
            ThrowItems(selectedQty);
            selectedQty = 1;
        }
    }

    private IEnumerator HandleHoldLogic()
    {
        // Wait to determine tap vs hold
        yield return new WaitForSeconds(0.25f);

        // If released before delay → quick throw 1 item
        if (!Input.GetKey(KeyCode.Q))
        {
            ThrowItems(1);
            yield break;
        }

        // Holding — show quantity selection UI
        if (throwUI != null)
            throwUI.SetActive(true);

        selectedQty = 1;
        UpdateThrowUI();

        while (isHoldingThrow)
        {

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f && Time.time - lastScrollTime > .02f)
            {
                var slot = InventoryManager.Instance.GetCurrentItem();
                if (slot == null || slot.itemData == null)
                    yield break;

                int max = slot.quantity;
                int dir = scroll > 0 ? 1 : -1;
                selectedQty = Mathf.Clamp(selectedQty + dir, 1, max);
                lastScrollTime = Time.time;

                UpdateThrowUI();
            }

            yield return null;
        }
    }

    private void UpdateThrowUI()
    {
        if (throwQtyText != null)
            throwQtyText.text = $"Throwing: {selectedQty}";
    }

    private void ThrowItems(int qty)
    {
        var slot = InventoryManager.Instance.GetCurrentItem();
        if (slot == null || slot.itemData == null)
        {
            UIManager.Instance?.ShowMessage("❌ No item selected to throw!");
            return;
        }

        var item = slot.itemData;

        // 🔹 Remove all selected quantity from inventory (logical)
        InventoryManager.Instance.RemoveItem(item, qty);

        // 🔹 If item count reached 0 — clear from hand
        var updatedSlot = InventoryManager.Instance.GetCurrentItem();
        if (updatedSlot == null || updatedSlot.itemData != item)
        {
            // Clear hand if this was the active item
            var visualizer = GetComponent<PlayerItemVisualizer>();
            if (visualizer != null)
            {
                visualizer.ClearCurrentItem();
            }
        }

        // 🔹 Throw visually only one physical object
        var obj = ObjectPoolManager.Instance.GetFromPool(item.handPoolKey);
        if (obj == null) return;

        obj.transform.position = throwOrigin.position;
        obj.transform.rotation = throwOrigin.rotation;

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.AddForce((throwOrigin.forward + Vector3.up * upwardModifier) * throwForce, ForceMode.Impulse);
        }

        if (obj.TryGetComponent(out Interactable interactable))
        {
            interactable.SetInteractable(true);
            if (interactable is PickableItem pickableItem)
            {
                pickableItem.SetItemCount(qty);
            }

        }
        if (obj.TryGetComponent(out ItemWorld iw))
            iw.Initialize(item);


        // 🔹 (Optional future feature: Attach floating text "xN")
        UIManager.Instance?.ShowMessage($"🪃 Threw {qty}x {item.itemName}");
    }
}
