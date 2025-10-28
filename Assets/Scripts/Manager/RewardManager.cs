using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    #region Singleton
    public static RewardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void GiveReward(RewardData reward)
    {
        if (reward == null)
        {
            Debug.LogWarning("지급할 RewardData가 없습니다.");
            return;
        }

        // 아이템 지급
        if (reward.itemRewards != null && reward.itemRewards.Count > 0)
        {
            foreach (var itemReward in reward.itemRewards)
            {
                // InventoryManager.AddItem() 호출
                InventoryManager.Instance.AddItem(itemReward.item, itemReward.amount);
                Debug.Log($"[RewardManager] 아이템 {itemReward.item.itemName} {itemReward.amount}개 획득!");
            }
        }
    }
}