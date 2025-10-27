using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuyerController : MonoBehaviour
{
    [Header("Buyer Setup")]
    [SerializeField] private BuyerDataSO buyerData;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform collectPoint; // Where item visually disappears
    [SerializeField] private float collectDelay = 0.2f;

    private int[] deliveredCounts;
    private BuyerInteractable buyerInteractable;
    private void Start()
    {
        if (buyerData == null)
        {
            Debug.LogError("BuyerController: Missing BuyerData!");
            return;
        }

        deliveredCounts = new int[buyerData.requirements.Length];
        buyerInteractable = GetComponent<BuyerInteractable>();
        buyerInteractable.PopulateRequirementContainer(buyerData.requirements);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if object thrown is an Interactable
        var interactable = collision.collider.GetComponent<Interactable>();
        if (interactable == null)
            return;

        var itemWorld = interactable.GetComponent<ItemWorld>();
        if (itemWorld == null || itemWorld.ItemData == null)
        {
            RejectItem(interactable.gameObject, "That's not something I buy!");
            return;
        }

        TryCollectItem(itemWorld);
    }

    private void TryCollectItem(ItemWorld item)
    {
        if (item == null || item.ItemData == null)
        {
            Debug.LogWarning("[BuyerController] Invalid or null item passed to TryCollectItem.");
            return;
        }

        for (int i = 0; i < buyerData.requirements.Length; i++)
        {
            Requirement req = buyerData.requirements[i];

            // ✅ Check if the item matches buyer’s requirement
            if (req.item == item.ItemData)
            {
                int currentDelivered = deliveredCounts[i];
                int requiredTotal = req.requiredQuantity;

                // Get how many units were actually thrown
                int thrownQuantity = 1;
                if (item.TryGetComponent<PickableItem>(out PickableItem pickable))
                    thrownQuantity = pickable.GetQuantity;

                int remainingNeeded = requiredTotal - currentDelivered;

                // ✅ Buyer already has enough
                if (remainingNeeded <= 0)
                {
                    RejectItem(item.gameObject, "I already have enough of this item!");
                    return;
                }

                // ✅ Determine accepted/rejected counts
                int accepted = Mathf.Min(thrownQuantity, remainingNeeded);
                int rejected = thrownQuantity - accepted;

                // Update delivered amount
                deliveredCounts[i] += accepted;

                // Return original thrown item to pool
                ObjectPoolManager.Instance.ReturnToPool(item.ItemData.handPoolKey, item.gameObject);

                // ✅ Handle leftovers in a single stacked object
                if (rejected > 0)
                {
                    Debug.Log($"[BuyerController] Buyer accepted {accepted}x but rejected {rejected}x {item.ItemData.itemName}");

                    var leftoverObj = ObjectPoolManager.Instance.GetFromPool(item.ItemData.handPoolKey);
                    if (leftoverObj != null)
                    {
                        leftoverObj.transform.position = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
                        leftoverObj.transform.rotation = Quaternion.identity;

                        // If it has PickableItem component, set stack quantity
                        if (leftoverObj.TryGetComponent(out PickableItem leftoverPickable))
                        {
                            leftoverPickable.SetItemCount(rejected);
                        }

                        if (leftoverObj.TryGetComponent(out Rigidbody rb))
                        {
                            rb.AddForce(Vector3.up * 2f + transform.forward * 1f, ForceMode.Impulse);
                        }
                    }
                }

                // ✅ Feedback
                if (audioSource && buyerData.acceptSound)
                    audioSource.PlayOneShot(buyerData.acceptSound);

                UIManager.Instance?.ShowMessage(
                    $"✅ {buyerData.buyerName}: Collected {accepted}x {req.item.itemName} ({deliveredCounts[i]}/{req.requiredQuantity})"
                );

                buyerInteractable.IncrementCurrentQuantity(req.requirementID, deliveredCounts[i]);

                CheckCompletion();
                return;
            }
        }

        // ❌ No requirement matched
        RejectItem(item.gameObject, "I don’t want this item!");
    }



    private void RejectItem(GameObject item, string message)
    {
        if (audioSource && buyerData.rejectSound)
            audioSource.PlayOneShot(buyerData.rejectSound);

        UIManager.Instance?.ShowMessage($"❌ {buyerData.buyerName}: {message}");

        // Optional: bounce back item
        if (item.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(Vector3.up * 4f + -transform.forward * 3f, ForceMode.Impulse);
        }
    }

    private void CheckCompletion()
    {
        for (int i = 0; i < buyerData.requirements.Length; i++)
        {
            if (deliveredCounts[i] < buyerData.requirements[i].requiredQuantity)
                return; // Still pending
        }

        // All requirements fulfilled
        UIManager.Instance?.ShowMessage($"🎉 {buyerData.buyerName}: Mission Complete!");

        HeatReputationManager.Instance.AddReputation(buyerData.rewardReputation);
        PlayerEconomy.Instance.AddMoney(buyerData.rewardMoney);

        Debug.Log($"{buyerData.buyerName} transaction complete!");
    }
}
