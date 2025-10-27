using UnityEngine;

public class CraftingTableInteractable : Interactable
{
    protected override void OnInteract()
    {
        CraftingManager.Instance.OpenCraftingTable();
    }

}
