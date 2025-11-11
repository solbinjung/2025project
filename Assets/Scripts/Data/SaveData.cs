using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string lastSafeSceneName;

    // 보유 아이템, 스킬,  퀘스트 진행 상황 데이터
    public List<ItemSlot> playerInventory;
    public List<QuestObjective> playerQuests;
    public List<SkillData> playerSkills;

    // 클래스 생성자 - 리스트 초기화
    public SaveData()
    {
        lastSafeSceneName = "MainScene";
        playerInventory = new List<ItemSlot>();
        playerQuests = new List<QuestObjective>();
        playerSkills = new List<SkillData>();
    }
}
