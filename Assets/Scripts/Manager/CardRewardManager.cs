using UnityEngine;
using System;

public class CardRewardManager : MonoBehaviour
{
    #region Singleton
    public static CardRewardManager Instance { get; private set; }
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

    //  현재 보유 중인 카드 뽑기권 개수
    [SerializeField] private int currentVouchers = 0;

    // 뽑기권 개수 변경 => UI에 이벤트 알림
    public static event Action<int> OnVoucherCountChanged;

    // RewardManager > 퀘스트 보상으로 뽑기권 추가
    public void AddVouchers(int amount)
    {
        if (amount <= 0) return;

        currentVouchers += amount;
        Debug.Log($"[CardRewardManager] 카드 뽑기권 {amount}개 획득! 총: {currentVouchers}개");

        OnVoucherCountChanged?.Invoke(currentVouchers);
    }

    // UI_CardButton => 카드 뽑기권 1개 사용
    public bool UseVoucher()
    {
        if (currentVouchers > 0)
        {
            currentVouchers--;
            Debug.Log($"[CardRewardManager] 카드 뽑기권 1개 사용. 남은 개수: {currentVouchers}개");

            OnVoucherCountChanged?.Invoke(currentVouchers);
            return true; // 사용 성공
        }
        else
        {
            Debug.LogWarning("[CardRewardManager] 사용할 카드 뽑기권이 없습니다!");
            return false; // 사용 실패
        }
    }

    // 현재 뽑기권 개수 확인
    public int GetCurrentVouchers()
    {
        return currentVouchers;
    }
}