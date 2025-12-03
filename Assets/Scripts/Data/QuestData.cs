using System.Collections.Generic;
using UnityEngine;

// 퀘스트 목표 타입
public enum ObjectiveType
{
    Kill,       // 몬스터 처치
    Collect,    // 아이템 수집
    Talk        // NPC와 대화
}

[System.Serializable]
public class QuestObjective
{
    public ObjectiveType type;
    public string description;

    [Header("Targets")]
    // Collect 타입
    public ItemData targetItem;

    // Kill 타입
    public int targetEnemyID;

    // Talk 타입
    public int targetNpcID;

    [Header("Amount")]
    public int requiredAmount;  // 필요한 수량 (대화는 보통 1)
    public int currentAmount;   // 현재 달성한 수량

    public bool IsCompleted => currentAmount >= requiredAmount;
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("Quest Info")]
    public int id;
    public string questName;
    [TextArea(3, 5)]
    public string description;

    [Header("Objectives")]
    public List<QuestObjective> objectives;

    [Header("Reward")]
    public RewardData reward;
}