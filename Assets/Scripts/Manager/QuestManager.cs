using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class ActiveQuest
{
    public QuestData data;

    // 실시간 진행도를 저장
    public List<QuestObjective> runtimeObjectives;

    // 목표 달성 후 NPC에게 보고하지 않은 상태
    public bool isReadyToComplete = false;

    public ActiveQuest(QuestData questData)
    {
        data = questData;

        runtimeObjectives = new List<QuestObjective>();
        foreach (var obj in questData.objectives)
        {
            runtimeObjectives.Add(new QuestObjective
            {
                type = obj.type,
                description = obj.description,
                targetItem = obj.targetItem,
                targetEnemyID = obj.targetEnemyID,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0 // 진행도 0으로 초기화
            });
        }
    }
    public bool IsAllObjectivesCompleted()
    {
        // 모든 목표가 완료되었는지 확인
        return runtimeObjectives.All(obj => obj.IsCompleted);
    }
}

public class QuestManager : MonoBehaviour
{
    #region Singleton
    public static QuestManager Instance { get; private set; }

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

    public static event Action OnQuestProgressChanged;

    [SerializeField] private QuestData finalBossQuest;

    // 퀘스트 진행 상황을 관리하는 리스트
    public List<ActiveQuest> activeQuests = new List<ActiveQuest>();
    public List<QuestData> completedQuests = new List<QuestData>();

    private void Start()
    {
        // ======퀘스트 종류======
        // 1. 인벤토리 이벤트 구독
        InventoryManager.OnInventoryChanged += OnItemAdded_CheckQuest;

        // 2. 몬스터 사망 이벤트 구독
        EnemyStats.OnEnemyDied += OnEnemyKilled_CheckQuest;

        // 3. NPC 대화 이벤트 구독
        // DialogueManager에서 이 이벤트를 방송
        DialogueManager.OnNpcTalked += OnNpcTalked_CheckQuest;
    }

    private void OnDestroy()
    {
        InventoryManager.OnInventoryChanged -= OnItemAdded_CheckQuest;
        EnemyStats.OnEnemyDied -= OnEnemyKilled_CheckQuest;
        DialogueManager.OnNpcTalked -= OnNpcTalked_CheckQuest;
    }

    public void ResetQuests()
    {
        Debug.Log("모든 퀘스트를 초기화합니다");

        // 모든 퀘스트 데이터 리스트를 비우기
        activeQuests.Clear();
        completedQuests.Clear();

        // 퀘스트 UI(UIQuestList) 새로고침
        OnQuestProgressChanged?.Invoke();
    }

    // 몬스터가 죽었을 때 호출될 함수
    public void OnEnemyKilled_CheckQuest(int enemyID)
    {
        bool changed = false;
        // 모든 진행 중인 퀘스트를 순회
        foreach (var quest in activeQuests)
        {
            // 퀘스트의 모든 목표를 순회
            foreach (var objective in quest.runtimeObjectives)
            {
                // "사냥" 타입이고 "ID가 일치"하면
                if (objective.type == ObjectiveType.Kill && objective.targetEnemyID == enemyID)
                {
                    objective.currentAmount++;
                    changed = true;
                    Debug.Log($"퀘스트 진행: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");
                    CheckIfReadyToComplete(quest);
                }
            }
        }
        if (changed) OnQuestProgressChanged?.Invoke();
    }

    // 인벤토리에 아이템이 추가될 때 호출될 함수
    private void OnItemAdded_CheckQuest()
    {
        bool changed = false;
        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.runtimeObjectives)
            {
                if (objective.type == ObjectiveType.Collect && !objective.IsCompleted)
                {
                    ItemSlot slot = InventoryManager.Instance.inventory.Find(s => s.item == objective.targetItem);
                    int currentAmountInInventory = (slot != null) ? slot.amount : 0;

                    if (objective.currentAmount != currentAmountInInventory)
                    {
                        objective.currentAmount = currentAmountInInventory;
                        changed = true;
                        Debug.Log($"퀘스트 진행: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");
                        CheckIfReadyToComplete(quest);
                    }
                }
            }
        }
        if (changed) OnQuestProgressChanged?.Invoke();
    }

    public void OnNpcTalked_CheckQuest(int npcID)
    {
        bool changed = false;

        // 모든 진행 중인 퀘스트를 순회
        foreach (var quest in activeQuests)
        {
            // 퀘스트의 모든 목표를 순회
            foreach (var objective in quest.runtimeObjectives)
            {
                if (objective.type == ObjectiveType.Talk &&
                    objective.targetNpcID == npcID &&
                    !objective.IsCompleted)
                {
                    objective.currentAmount++; // 대화 횟수 1 증가
                    changed = true;
                    Debug.Log($"퀘스트 진행: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");

                    CheckIfReadyToComplete(quest);
                }
            }
        }
        if (changed) OnQuestProgressChanged?.Invoke();
    }
    // 퀘스트를 수락하는 함수
    public void AcceptQuest(QuestData questData)
    {
        if (completedQuests.Contains(questData) || activeQuests.Any(q => q.data == questData))
        {
            Debug.LogWarning($"이미 수락했거나 완료한 퀘스트입니다: {questData.questName}");
            return;
        }

        ActiveQuest newQuest = new ActiveQuest(questData);
        activeQuests.Add(newQuest);
        Debug.Log($"퀘스트 수락: {questData.questName}");

        OnQuestProgressChanged?.Invoke();
    }

    // 특정 퀘스트가 완료되었는지 확인
    private void CheckIfReadyToComplete(ActiveQuest quest)
    {
        if (quest.isReadyToComplete || !quest.IsAllObjectivesCompleted())
        {
            return;
        }

        quest.isReadyToComplete = true;
        Debug.LogWarning($"퀘스트 목표 달성: {quest.data.questName}. NPC에게 돌아가세요.");

        OnQuestProgressChanged?.Invoke();
    }

    public void CompleteQuest(QuestData questData)
    {
        // 진행 중인 퀘스트 리스트에서 해당 퀘스트를 찾음
        ActiveQuest questToComplete = activeQuests.Find(q => q.data == questData);

        if (questToComplete == null)
        {
            Debug.LogWarning($"완료하려는 퀘스트({questData.questName})가 진행 중이지 않습니다.");
            return;
        }

        // 모든 목표가 완료되었는지 확인
        if (!questToComplete.IsAllObjectivesCompleted())
        {
            Debug.LogWarning("퀘스트 목표가 아직 완료되지 않았습니다.");
            return;
        }

        // 보상 지급
        RewardManager.Instance.GiveReward(questToComplete.data.reward);

        // 퀘스트 목록 변경(완료 퀘스트 제거)
        activeQuests.Remove(questToComplete);
        completedQuests.Add(questToComplete.data);

        Debug.Log($"[QuestManager] 퀘스트 완료: {questToComplete.data.questName}");

        // 최종 퀘스트 후 게임 엔딩
        if (questData == finalBossQuest && UIManager.Instance != null)
        {
            Debug.Log("게임 완료 엔딩");
            UIManager.Instance.ShowGameEndingPanel();
        }

        OnQuestProgressChanged?.Invoke();

        // 게임 저장
        Debug.Log("저장 중...");
        SaveLoadManager.Instance.SaveGame();
    }

    public void LoadQuestState(List<int> savedCompletedIDs, List<ActiveQuestSaveData> savedActiveQuests, List<QuestData> questDatabase)
    {
        // 초기화
        activeQuests.Clear();
        completedQuests.Clear();

        // 완료된 퀘스트 복구
        foreach (int id in savedCompletedIDs)
        {
            QuestData data = questDatabase.Find(x => x.id == id);
            if (data != null)
            {
                completedQuests.Add(data);
            }
        }

        // 진행 중인 퀘스트 복구
        foreach (var saveData in savedActiveQuests)
        {
            QuestData data = questDatabase.Find(x => x.id == saveData.questID);
            if (data != null)
            {
                // 새 활성 퀘스트 생성
                ActiveQuest newQuest = new ActiveQuest(data);

                newQuest.isReadyToComplete = saveData.isReadyToComplete;

                // 각 목표(Objective)의 진행 수치(currentAmount) 덮어쓰기
                if (saveData.objectiveProgressCounts != null && saveData.objectiveProgressCounts.Count == newQuest.runtimeObjectives.Count)
                {
                    for (int i = 0; i < newQuest.runtimeObjectives.Count; i++)
                    {
                        newQuest.runtimeObjectives[i].currentAmount = saveData.objectiveProgressCounts[i];
                    }
                }

                activeQuests.Add(newQuest);
            }
        }

        // 4. UI 갱신
        OnQuestProgressChanged?.Invoke();
        Debug.Log($"로드 완료");
    }
}