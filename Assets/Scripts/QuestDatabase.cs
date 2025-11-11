using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestDatabase : MonoBehaviour
{
    public static QuestDatabase Instance { get; private set; }

    [SerializeField] private List<QuestData> allQuests;

    private Dictionary<string, QuestData> questDictionary = new Dictionary<string, QuestData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 딕셔너리 생성
            foreach (QuestData quest in allQuests)
            {
                if (quest != null && !questDictionary.ContainsKey(quest.name))
                {
                    questDictionary.Add(quest.name, quest);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public QuestData GetQuestByName(string questName)
    {
        if (string.IsNullOrEmpty(questName)) return null;

        QuestData questData;
        if (questDictionary.TryGetValue(questName, out questData))
        {
            return questData;
        }

        Debug.LogWarning($"[QuestDatabase] '{questName}' 이름을 가진 퀘스트를 찾을 수 없습니다!");
        return null;
    }
}