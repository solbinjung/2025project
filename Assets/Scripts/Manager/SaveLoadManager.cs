using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

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
}