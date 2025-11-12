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
                currentAmount = 0 // 진행도는 0에서 시작
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
        // 1. 인벤토리 이벤트 구독
        InventoryManager.OnInventoryChanged += OnItemAdded_CheckQuest;

        // 2. 몬스터 사망 이벤트 구독
        EnemyStats.OnEnemyDied += OnEnemyKilled_CheckQuest;

        // 3. NPC 대화 이벤트 구독
        // (DialogueManager 같은 다른 스크립트에서 이 이벤트를 방송해야 함)
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
        Debug.Log("[QuestManager] 모든 퀘스트를 초기화합니다.");

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
                    Debug.Log($"[QuestManager] 퀘스트 진행: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");
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
                        Debug.Log($"[QuestManager] 퀘스트 진행: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");
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
                    Debug.Log($"[QuestManager] 퀘스트 진행: {objective.description} ({objective.currentAmount}/{objective.requiredAmount})");

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
        Debug.Log($"[QuestManager] 퀘스트 수락: {questData.questName}");

        OnQuestProgressChanged?.Invoke();
    }

    // 특정 퀘스트가 완료되었는지 확인
    private void CheckIfReadyToComplete(ActiveQuest quest)
    {
        // 이미 보고 가능 상태이거나, 아직 목표 달성을 못했으면 아무것도 안 함
        if (quest.isReadyToComplete || !quest.IsAllObjectivesCompleted())
        {
            return;
        }

        quest.isReadyToComplete = true;
        Debug.LogWarning($"[QuestManager] 퀘스트 목표 달성!: {quest.data.questName}. NPC에게 돌아가세요.");

        OnQuestProgressChanged?.Invoke();
    }

    public void CompleteQuest(QuestData questData)
    {
        // 1. 진행 중인 퀘스트 리스트에서 해당 퀘스트를 찾음
        ActiveQuest questToComplete = activeQuests.Find(q => q.data == questData);

        if (questToComplete == null)
        {
            Debug.LogWarning($"완료하려는 퀘스트({questData.questName})가 진행 중이지 않습니다.");
            return;
        }

        // 2. 모든 목표가 완료되었는지 확인
        if (!questToComplete.IsAllObjectivesCompleted())
        {
            Debug.LogWarning("퀘스트 목표가 아직 완료되지 않았습니다.");
            return;
        }

        // 3. 보상 지급
        RewardManager.Instance.GiveReward(questToComplete.data.reward);

        // 4. 퀘스트 목록 변경
        activeQuests.Remove(questToComplete);
        completedQuests.Add(questToComplete.data);

        Debug.Log($"[QuestManager] 퀘스트 완료: {questToComplete.data.questName}");

        if (questData == finalBossQuest && UIManager.Instance != null)
        {
            Debug.Log("게임 완료 엔딩");
            UIManager.Instance.ShowGameEndingPanel();
        }

        OnQuestProgressChanged?.Invoke();
    }
}