using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using System.Text; 

public class UIQuestList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questLogText; 

    void Start()
    {
        if (questLogText == null)
        {
            questLogText = GetComponent<TextMeshProUGUI>();
        }

        // 1. 퀘스트 매니저의 이벤트에 UpdateQuestLog 함수를 구독
        QuestManager.OnQuestProgressChanged += UpdateQuestLog;

        // 2. 게임 시작 시 현재 퀘스트 목록으로 초기화
        UpdateQuestLog();
    }

    private void OnDestroy()
    {
        // 3. 오브젝트 파괴 시 이벤트 구독 해제 (메모리 누수 방지)
        QuestManager.OnQuestProgressChanged -= UpdateQuestLog;
    }

    /// <summary>
    /// 퀘스트 매니저가 변경될 때마다 호출되어 UI 텍스트를 새로고침
    /// </summary>
    void UpdateQuestLog()
    {
        // 1. QuestManager에서 현재 진행 중인 퀘스트 목록을 가져옴
        List<ActiveQuest> activeQuests = QuestManager.Instance.activeQuests;

        // 2. 텍스트를 효율적으로 만들기 위한 StringBuilder 사용
        StringBuilder sb = new StringBuilder();

        // 3. 퀘스트가 하나도 없으면
        if (activeQuests.Count == 0)
        {
            sb.Append("진행 중인 퀘스트가 없습니다.");
        }
        else
        {
            // 4. 모든 퀘스트를 순회
            foreach (var quest in activeQuests)
            {
                // 퀘스트 제목 추가
                sb.AppendLine($"<b>{quest.data.questName}</b>"); // <b> 태그로 굵게

                // 퀘스트의 모든 목표를 순회
                foreach (var objective in quest.runtimeObjectives)
                {
                    // 5."블루 드래곤 처치 (1/3)" 형식으로 목표 추가
                    sb.AppendLine($"  - {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");
                }
                sb.AppendLine(); // 퀘스트 사이에 한 줄 띄우기
            }
        }

        // 6. 최종 텍스트를 UI에 적용
        questLogText.text = sb.ToString();
    }
}