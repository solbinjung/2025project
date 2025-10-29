using UnityEngine;
using System; // Action (이벤트) 사용

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

    //  현재 보유 중인 '카드 뽑기권' 개수
    [SerializeField] private int currentVouchers = 0;

    // 뽑기권 개수가 변경될 때마다 UI에 알릴 이벤트
    public static event Action<int> OnVoucherCountChanged;

    /// <summary>
    /// (RewardManager가 호출) 퀘스트 보상으로 뽑기권 추가
    /// </summary>
    public void AddVouchers(int amount)
    {
        if (amount <= 0) return;

        currentVouchers += amount;
        Debug.Log($"[CardRewardManager] 카드 뽑기권 {amount}개 획득! 총: {currentVouchers}개");

        // "뽑기권 개수가 바뀌었다!"고 방송
        OnVoucherCountChanged?.Invoke(currentVouchers);
    }

    /// <summary>
    /// (UI_CardButton이 호출) 카드 뽑기권 1개 사용
    /// </summary>
    /// <returns>사용에 성공하면 true, 뽑기권이 없으면 false</returns>
    public bool UseVoucher()
    {
        if (currentVouchers > 0)
        {
            currentVouchers--;
            Debug.Log($"[CardRewardManager] 카드 뽑기권 1개 사용. 남은 개수: {currentVouchers}개");

            // "뽑기권 개수가 바뀌었다!"고 방송
            OnVoucherCountChanged?.Invoke(currentVouchers);
            return true; // 사용 성공
        }
        else
        {
            Debug.LogWarning("[CardRewardManager] 사용할 카드 뽑기권이 없습니다!");
            return false; // 사용 실패
        }
    }

    /// <summary>
    /// (UI가 호출) 현재 뽑기권 개수 확인
    /// </summary>
    public int GetCurrentVouchers()
    {
        return currentVouchers;
    }
}