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

    private PlayerSkillManager playerSkillManager;

    private void Start()
    {
        playerSkillManager = FindObjectOfType<PlayerSkillManager>();

        if (playerSkillManager == null)
        {
            Debug.LogError("[RewardManager] PlayerSkillManager를 씬에서 찾을 수 없습니다!");
        }
    }

    public void GiveReward(RewardData reward)
    {
        if (reward == null)
        {
            Debug.LogWarning("지급할 RewardData가 없습니다.");
            return;
        }

        if (reward.itemRewards != null && reward.itemRewards.Count > 0)
        {
            foreach (var itemReward in reward.itemRewards)
            {
                InventoryManager.Instance.AddItem(itemReward.item, itemReward.amount);
                Debug.Log($"[RewardManager] 아이템 {itemReward.item.itemName} {itemReward.amount}개 획득!");
            }
        }

        if (reward.cardDrawVouchers > 0)
        {
            CardRewardManager.Instance.AddVouchers(reward.cardDrawVouchers);
        }
    }
}
