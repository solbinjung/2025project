using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlotData
{
    public string itemName;
    public int amount;

    public ItemSlotData(string name, int num)
    {
        itemName = name;
        amount = num;
    }
}
[System.Serializable]
public class ActiveQuestData
{
    public string questDataName; 
    public List<QuestObjective> runtimeObjectives; 
    public bool isReadyToComplete;

    public ActiveQuestData(string name, List<QuestObjective> objectives, bool isReady)
    {
        questDataName = name;
        runtimeObjectives = objectives;
        isReadyToComplete = isReady;
    }
}
[System.Serializable]
public class SaveData
{
    public string lastSafeSceneName;

    // 보유 아이템, 스킬,  퀘스트 진행 상황 데이터
    public List<ItemSlotData> playerInventory;

    public List<ActiveQuestData> activeQuests;
    public List<string> completedQuestNames;

    public List<SkillData> playerSkills;

    // 클래스 생성자 - 리스트 초기화
    public SaveData()
    {
        lastSafeSceneName = "MainScene";

        playerInventory = new List<ItemSlotData>();

        activeQuests = new List<ActiveQuestData>();
        completedQuestNames = new List<string>();

        playerSkills = new List<SkillData>();
    }
}
