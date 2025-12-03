using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillSlotSaveData
{
    public KeyCode key;
    public int skillID;
}

[System.Serializable]
public struct ActiveQuestSaveData
{
    public int questID;
    public bool isReadyToComplete;
    public List<int> objectiveProgressCounts; // 목표별 달성 수치
}

[System.Serializable]
public class SaveData
{
    // 기본 정보
    public string sceneName;       // 저장된 씬 이름
    public Vector3 playerPosition; // 플레이어 위치

    // 플레이어 상태
    public int currentHp;
    public int maxHp;
    public int currentMp;
    public int maxMp;

    // 3. 인벤토리: ID와 개수 리스트로 분리하여 저장
    public List<int> invItemIDs = new List<int>();      // 아이템 ID
    public List<int> invItemAmounts = new List<int>();  // 개수

    // 진행중인 퀘스트 ID
    public List<int> completedQuestIDs = new List<int>(); // 완료된 퀘스트 ID 목록
    public List<ActiveQuestSaveData> activeQuests = new List<ActiveQuestSaveData>(); // 진행중인 퀘스트 목록

    // 보유한 스킬 ID
    public List<int> ownedSkillIDs = new List<int>();
    public List<SkillSlotSaveData> skillSlots = new List<SkillSlotSaveData>();
}