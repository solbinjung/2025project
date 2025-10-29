using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICardButton : MonoBehaviour
{
    [SerializeField] private Button cardButton; // Inspector에서 "카드 버튼" 자체를 연결
    [SerializeField] private TextMeshProUGUI countText; // Inspector에서 개수 표시 텍스트 연결

    // Inspector에서 'SkillDrawUIManager' 오브젝트를 연결
    [SerializeField] private SkillDrawUIManager skillDrawUIManager;

    void Start()
    {
        if (cardButton == null)
            cardButton = GetComponent<Button>();
        if (countText == null)
            countText = GetComponentInChildren<TextMeshProUGUI>();

        // 1. (중요) 버튼 클릭 시 OnClickButton 함수가 실행되도록 등록
        cardButton.onClick.AddListener(OnClickButton);

        // 2. (중요) 카드 뽑기권 개수 변경 이벤트 구독
        CardRewardManager.OnVoucherCountChanged += UpdateVoucherCount;

        // 3. (중요) 게임 시작 시 초기 개수/상태로 업데이트
        UpdateVoucherCount(CardRewardManager.Instance.GetCurrentVouchers());
    }

    private void OnDestroy()
    {
        // 4. 이벤트 구독 해제
        CardRewardManager.OnVoucherCountChanged -= UpdateVoucherCount;
    }

    /// <summary>
    /// CardRewardManager가 호출할 이벤트 핸들러
    /// </summary>
    void UpdateVoucherCount(int count)
    {
        // 1. 텍스트 업데이트
        countText.text = count.ToString();

        // 2. (핵심) 뽑기권이 0개이면 버튼 비활성화, 1개 이상이면 활성화
        cardButton.interactable = (count > 0);
    }

    /// <summary>
    /// 플레이어가 이 버튼을 클릭했을 때 호출
    /// </summary>
    void OnClickButton()
    {
        // 1. 카드 뽑기권이 있는지 다시 확인
        if (CardRewardManager.Instance.GetCurrentVouchers() > 0)
        {
            // 2. 뽑기권 1개 사용 시도
            bool success = CardRewardManager.Instance.UseVoucher();

            // 3. 사용에 성공하면
            if (success)
            {
                // 4. 랜덤 카드 뽑기 UI를 켬
                skillDrawUIManager.OpenSkillDrawPanel();
            }
        }
    }
}
