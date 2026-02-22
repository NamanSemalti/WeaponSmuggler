using UnityEngine;

[CreateAssetMenu(menuName = "WeaponSmuggler/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    public ItemDataSO item;
    public int pricePerUnit;
    [TextArea] public string description;
}
