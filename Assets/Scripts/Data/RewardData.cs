using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemReward
{
    public ItemData item;
    public int amount;
}

[CreateAssetMenu(fileName = "New Reward", menuName = "Quest/RewardData")]
public class RewardData : ScriptableObject
{
    public List<ItemReward> itemRewards;
}