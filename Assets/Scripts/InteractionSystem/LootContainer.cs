using UnityEngine;

public class LootContainer : Interactable
{
    [Header("Loot Container Settings")]
    [SerializeField] private bool canBeOpened = true;
    [SerializeField] private bool destroyAfterOpen = false;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private Animator animator;
    [SerializeField] private string openParameter = "IsOpen";

    [Header("Loot Table Reference")]
    [SerializeField] private LootTableSO lootTable;
    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 3;

    public bool _isOpened = false;


    protected override void OnInteract()
    {
        if (_isOpened || !canBeOpened) return;

        _isOpened = true;

        if (openSound)
            AudioSource.PlayClipAtPoint(openSound, transform.position);

        if (animator && !string.IsNullOrEmpty(openParameter))
            animator.SetBool(openParameter, true);

        InteractionEvents.TriggerUse(this);

        SpawnLoot();

        if (destroyAfterOpen)
            Destroy(gameObject, 1f);
    }

    private void SpawnLoot()
    {
        if (lootTable == null)
        {
            Debug.LogWarning($"{gameObject.name} has no LootTableSO assigned!");
            return;
        }

        int lootCount = Random.Range(minItems, maxItems + 1);

        for (int i = 0; i < lootCount; i++)
        {
            LootEntry entry = lootTable.GetRandomLoot();
            if (entry != null && entry.prefab != null)
            {
                Vector3 dropPos = transform.position + new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0.5f,
                    Random.Range(-0.5f, 0.5f)
                );

                Instantiate(entry.prefab, dropPos, Quaternion.identity);
                Debug.Log($"Spawned loot: {entry.itemName}");
            }
        }
    }
}
