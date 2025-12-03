using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    public List<SkillData> allSkillDatabase; // 스킬 DB
    public List<ItemData> allItemDatabase;   // 아이템 DB
    public List<QuestData> allQuestDatabase; // 퀘스트 DB

    private string saveFilePath;
    public SaveData currentSaveData; // 현재 메모리에 로드된 데이터

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 저장
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 1. 씬 이름 저장
        data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // 2. 플레이어 정보 수집
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position;
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                data.currentHp = stats.CurrentHp;
                data.currentMp = stats.CurrentMp;
            }
        }

        // 인벤토리 저장
        if (InventoryManager.Instance != null)
        {
            foreach (ItemSlot slot in InventoryManager.Instance.inventory)
            {
                if (slot.item == null)
                {
                    // 빈 슬롯은 ID를 -1로 저장
                    data.invItemIDs.Add(-1);
                    data.invItemAmounts.Add(0);
                }
                else
                {
                    // 아이템이 있으면 ID와 개수 저장
                    data.invItemIDs.Add(slot.item.id);
                    data.invItemAmounts.Add(slot.amount);
                }
            }
        }

        // 퀘스트 저장
        if (QuestManager.Instance != null)
        {
            // 완료된 퀘스트 ID 저장
            foreach (var qData in QuestManager.Instance.completedQuests)
            {
                data.completedQuestIDs.Add(qData.id);
            }

            // 진행 중인 퀘스트 저장
            foreach (var activeQ in QuestManager.Instance.activeQuests)
            {
                ActiveQuestSaveData qSaveData = new ActiveQuestSaveData();
                qSaveData.questID = activeQ.data.id;
                qSaveData.isReadyToComplete = activeQ.isReadyToComplete;
                qSaveData.objectiveProgressCounts = new List<int>();

                // 각 목표의 현재 수치를 리스트에 담기
                foreach (var obj in activeQ.runtimeObjectives)
                {
                    qSaveData.objectiveProgressCounts.Add(obj.currentAmount);
                }
                data.activeQuests.Add(qSaveData);
            }
        }

        // 5. 스킬 저장
        if (PlayerSkillManager.Instance != null)
        {
            foreach (var skill in PlayerSkillManager.Instance.ownedSkills)
            {
                data.ownedSkillIDs.Add(skill.id);
            }
            foreach (var pair in PlayerSkillManager.Instance.skillMap)
            {
                SkillSlotSaveData slotData = new SkillSlotSaveData();
                slotData.key = pair.Key;
                slotData.skillID = pair.Value.id;
                data.skillSlots.Add(slotData);
            }
        }

        // JSON 변환 및 파일 쓰기
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"게임 저장 완료: {saveFilePath}");
    }

    // 불러오기
    public bool LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("저장된 파일이 없습니다.");
            return false;
        }

        string json = File.ReadAllText(saveFilePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);

        return true;
    }

    public void ApplyPlayerHandler()
    {
        if (currentSaveData == null) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerStats>()?.LoadStatus(currentSaveData.currentHp, currentSaveData.currentMp, currentSaveData.playerPosition);
        }
    }
    public void ApplySkillHandler()
    {
        if (currentSaveData == null) return;
        if (PlayerSkillManager.Instance == null) return;

        List<SkillData> loadedOwned = new List<SkillData>();
        Dictionary<KeyCode, SkillData> loadedMap = new Dictionary<KeyCode, SkillData>();

        // ID로 배운 스킬 찾기
        foreach (int id in currentSaveData.ownedSkillIDs)
        {
            SkillData skill = allSkillDatabase.Find(x => x.id == id);
            if (skill != null) loadedOwned.Add(skill);
        }

        // ID로 슬롯 배치 찾기
        foreach (var slotData in currentSaveData.skillSlots)
        {
            SkillData skill = allSkillDatabase.Find(x => x.id == slotData.skillID);
            if (skill != null)
            {
                loadedMap[slotData.key] = skill;
            }
        }

        // 스킬매니저에게 전달
        PlayerSkillManager.Instance.LoadSkills(loadedOwned, loadedMap);
    }
    public void ApplyInventoryHandler()
    {
        if (currentSaveData == null) return;
        if (InventoryManager.Instance == null) return;

        // InventoryManager에 ID 리스트, 개수 리스트, 아이템 DB 전달
        InventoryManager.Instance.LoadInventory(
            currentSaveData.invItemIDs,
            currentSaveData.invItemAmounts,
            allItemDatabase
        );
    }
    public void ApplyQuestHandler()
    {
        if (currentSaveData == null || QuestManager.Instance == null) return;

        QuestManager.Instance.LoadQuestState(
            currentSaveData.completedQuestIDs,
            currentSaveData.activeQuests,
            allQuestDatabase
        );
    }

    // 새 게임
    public void StartNewGame()
    {
        Debug.Log("새 게임 시작. PlayerPrefs 데이터 삭제.");

        // 인벤토리 매니저 초기화
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.inventory.Clear();
            InventoryManager.Instance.InitializeInventory();
        }
        else
        {
            Debug.LogWarning("InventoryManager not found for reset.");
        }

        // 퀘스트 매니저 초기화
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ResetQuests();
        }
        else
        {
            Debug.LogWarning("QuestManager not found for reset.");
        }

        // 스킬 매니저 초기화
        if (PlayerSkillManager.Instance != null)
        {
            PlayerSkillManager.Instance.ResetSkills();
        }
        // UI 새로고침
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.RedrawAll();
        }
    }

    // 데이터 존재 여부
    public bool HasSaveData
    {
        get { return System.IO.File.Exists(saveFilePath); }
    }
}