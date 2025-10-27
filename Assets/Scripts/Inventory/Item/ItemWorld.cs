using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public ItemDataSO ItemData { get; private set; }

    public void Initialize(ItemDataSO item)
    {
        ItemData = item;
    }
}
