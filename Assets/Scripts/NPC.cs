using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // 퀘스트 데이터의 targetNpcID와 동일
    public int npcID;

    public void OnInteract()
    {
        // 대화 시작
        DialogueManager.Instance.StartDialogue(npcID);

        // TODO: (선택) 퀘스트 수락/완료 UI 띄우기
        // QuestManager.Instance.AcceptQuest(...)
        // QuestManager.Instance.CompleteQuest(...)
    }
}