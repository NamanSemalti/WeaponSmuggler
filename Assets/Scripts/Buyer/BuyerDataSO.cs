using UnityEngine;
[System.Serializable]
public class Requirement
{
    public string requirementID = System.Guid.NewGuid().ToString();
    public ItemDataSO item;
    public int requiredQuantity = 1;
}

[CreateAssetMenu(fileName = "BuyerData", menuName = "Game/BuyerData")]
public class BuyerDataSO : ScriptableObject
{
    [Header("Buyer Configuration")]
    public string buyerName;
    public Requirement[] requirements;
    public int rewardMoney = 100;
    public int rewardReputation = 5;
    public AudioClip acceptSound;
    public AudioClip rejectSound;
}
