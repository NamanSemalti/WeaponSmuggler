using UnityEngine;

public class PlayerItemVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform handMount;
    private GameObject currentItemObject;
    private string currentPoolKey;
    private ItemDataSO currentItem;

    void Start()
    {
        InventoryManager.Instance.onItemSelected += OnItemSelected;
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.onItemSelected -= OnItemSelected;
    }

    private void OnItemSelected(InventorySlot slot)
    {
        ClearCurrentItem();

        if (slot == null || slot.itemData == null)
        {
            currentItem = null;
            return;
        }

        currentItem = slot.itemData;
        currentPoolKey = currentItem.handPoolKey;

        if (!string.IsNullOrEmpty(currentPoolKey))
        {
            currentItemObject = ObjectPoolManager.Instance.GetFromPool(currentPoolKey, handMount);
            if (currentItemObject == null) return;

            if (currentItemObject.TryGetComponent(out Interactable i))
                i.SetInteractable(false);

            currentItemObject.transform.localPosition = currentItem.handOffset;
            currentItemObject.transform.localEulerAngles = currentItem.handRotation;
            currentItemObject.transform.localScale = currentItem.handScale;
        }
    }

    public void ClearCurrentItem()
    {
        if (currentItemObject != null && !string.IsNullOrEmpty(currentPoolKey))
        {
            ObjectPoolManager.Instance.ReturnToPool(currentPoolKey, currentItemObject);
            currentItemObject = null;
        }

        currentItem = null;
        currentPoolKey = string.Empty;
    }
}
