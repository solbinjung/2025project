using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private const string SaveKey = "myGameSaveData";

    private SaveData currentSaveData;

    void Awake()
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

    // 새 게임
    public void StartNewGame()
    {
        Debug.Log("새 게임 시작. PlayerPrefs 데이터 삭제.");

        // PlayerPrefs에서 해당 키의 데이터를 삭제
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        // 현재 메모리 데이터 초기화
        currentSaveData = new SaveData();

        // 실제 게임 매니저들 초기화
        InventoryManager.Instance.inventory.Clear();
        InventoryManager.Instance.InitializeInventory();
        QuestManager.Instance.activeQuests.Clear();
        QuestManager.Instance.completedQuests.Clear();
        PlayerSkillManager.Instance.ownedSkills.Clear();

        // UI 새로고침
        if (InventoryUIManager.Instance != null)
            InventoryUIManager.Instance.RedrawAll();
    }

    // 자동 저장
    public void AutosaveGame(string currentSafeSceneName)
    {
        if (currentSaveData == null)
        {
            currentSaveData = new SaveData();
        }

        Debug.Log(currentSafeSceneName + "을(를) 안전 지점으로 저장합니다.");

        currentSaveData.lastSafeSceneName = currentSafeSceneName;
        
        // 아이템
        currentSaveData.playerInventory.Clear();
        foreach (ItemSlot slot in InventoryManager.Instance.inventory)
        {
            if (slot.item != null)
                currentSaveData.playerInventory.Add(new ItemSlotData(slot.item.name, slot.amount));
            else
                currentSaveData.playerInventory.Add(new ItemSlotData(null, 0));
        }
        
        // 진행 중인 퀘스트
        currentSaveData.activeQuests.Clear();
        foreach (ActiveQuest activeQuest in QuestManager.Instance.activeQuests)
        {
            if (activeQuest.data != null)
            {
                currentSaveData.activeQuests.Add(new ActiveQuestData(
                    activeQuest.data.name,
                    activeQuest.runtimeObjectives,
                    activeQuest.isReadyToComplete
                ));
            }
        }

        // 완료 퀘스트
        currentSaveData.completedQuestNames.Clear();
        foreach (QuestData completedQuest in QuestManager.Instance.completedQuests)
        {
            if (completedQuest != null)
            {
                currentSaveData.completedQuestNames.Add(completedQuest.name);
            }
        }
        currentSaveData.playerSkills = PlayerSkillManager.Instance.ownedSkills;

        // SaveData 객체를 통째로 JSON 문자열(string)로 변환
        string json = JsonUtility.ToJson(currentSaveData);

        // 파일에 쓰는 대신, PlayerPrefs에 "SaveKey"라는 이름으로 통째로 저장
        PlayerPrefs.SetString(SaveKey, json);

        // 변경 사항을 물리적으로 저장
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs 자동 저장 완료.");
    }

    // 게임 불러오기
    public void LoadGame()
    {
        // PlayerPrefs에서 "SaveKey"로 문자열을 가져옴
        string json = PlayerPrefs.GetString(SaveKey, null);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("불러올 세이브 파일이 없습니다!");
            return;
        }

        Debug.Log("PlayerPrefs 불러오기 시작...");

        // Json을 SaveData 객체로 변환
        currentSaveData = JsonUtility.FromJson<SaveData>(json);

        List<ItemSlot> loadedInventory = new List<ItemSlot>();
        foreach (ItemSlotData slotData in currentSaveData.playerInventory)
        {
            if (!string.IsNullOrEmpty(slotData.itemName))
            {
                ItemData itemRef = ItemDatabase.Instance.GetItemByName(slotData.itemName);
                loadedInventory.Add(new ItemSlot(itemRef, slotData.amount));
            }
            else
            {
                loadedInventory.Add(new ItemSlot(null, 0));
            }
        }
        InventoryManager.Instance.inventory = loadedInventory;

        List<ActiveQuest> loadedActiveQuests = new List<ActiveQuest>();
        foreach (ActiveQuestData questData in currentSaveData.activeQuests)
        {
            // QuestDatabase를 사용해 string 이름을 QuestData 참조로 변환
            QuestData questRef = QuestDatabase.Instance.GetQuestByName(questData.questDataName);
            if (questRef != null)
            {
                // ActiveQuest 생성자로 실제 데이터 복원
                ActiveQuest restoredQuest = new ActiveQuest(questRef);

                restoredQuest.runtimeObjectives = questData.runtimeObjectives;
                restoredQuest.isReadyToComplete = questData.isReadyToComplete;

                loadedActiveQuests.Add(restoredQuest);
            }
        }
        // 변환된 리스트를 QuestManager에 덮어쓰기
        QuestManager.Instance.activeQuests = loadedActiveQuests;

        List<QuestData> loadedCompletedQuests = new List<QuestData>();
        foreach (string questName in currentSaveData.completedQuestNames)
        {
            QuestData questRef = QuestDatabase.Instance.GetQuestByName(questName);
            if (questRef != null)
            {
                loadedCompletedQuests.Add(questRef);
            }
        }
        QuestManager.Instance.completedQuests = loadedCompletedQuests;

        PlayerSkillManager.Instance.ownedSkills = currentSaveData.playerSkills;

        // UI 새로고침
        if (InventoryUIManager.Instance != null)
            InventoryUIManager.Instance.RedrawAll();

        // 씬 로드
        UIManager.Instance.LoadSceneWithLoadingScreen(currentSaveData.lastSafeSceneName);
    }

    public bool CheckIfSaveDataExists()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }
}