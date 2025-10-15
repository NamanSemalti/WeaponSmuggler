using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private ItemDatabaseSO itemDatabaseAsset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static ItemDataSO GetItemByID(string id)
    {
        if (Instance == null || Instance.itemDatabaseAsset == null)
        {
            Debug.LogError("ItemDatabase instance or asset not set up!");
            return null;
        }

        return Instance.itemDatabaseAsset.GetItemByID(id);
    }
}
