using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    #endregion

    public static event Action<int> OnNpcTalked;

    public void StartDialogue(int npcID)
    {
        Debug.Log($"NPC {npcID}와 대화 시작...");
        EndDialogue(npcID);
    }

    private void EndDialogue(int npcID)
    {
        Debug.Log($"NPC {npcID}와 대화 종료.");

        OnNpcTalked?.Invoke(npcID);
    }
}