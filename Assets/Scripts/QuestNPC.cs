using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro; 

public class QuestNPC : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestData questToGive;
    [SerializeField] private string npcName = "마을 주민";

    [Header("UI")]
    [SerializeField] private GameObject interactPrompt;
    
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton; 

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera dialogueCamera;
    
    private bool playerIsNearby = false;
    private int defaultPriority;

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        if (dialogueCamera != null)
        {
            defaultPriority = dialogueCamera.Priority;
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            if (nextButton != null)
            {
                // 1. 기존에 Inspector에 설정된 리스너가 있다면 모두 제거
                nextButton.onClick.RemoveAllListeners();
                // 2. CloseDialogue 함수를 클릭 이벤트에 등록
                nextButton.onClick.AddListener(CloseDialogue);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
    private void Update()
    {
        // 플레이어가 근처에 있고 E키를 눌렀다면
        if (playerIsNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    private void Interact()
    {
        StartDialogueCamera();

        QuestManager qm = QuestManager.Instance;

        // 1. 현재 퀘스트가 '진행 중'인지 확인
        ActiveQuest activeQuest = qm.activeQuests.Find(q => q.data == questToGive);

        if (activeQuest != null) // 퀘스트가 '진행 중'일 때
        {
            if (activeQuest.IsAllObjectivesCompleted())
            {
                // 2. (A) 목표 달성 -> 퀘스트 완료
                qm.CompleteQuest(questToGive);
                ShowDialogue("감사합니다! 여기 보상입니다.");
            }
            else
            {
                // 3. (B) 목표 미달성 -> 진행 중 대화
                ShowDialogue("아직 임무를 완료하지 못했군요. 어서 가보세요.");
            }
        }
        // 4. 퀘스트가 '이미 완료'되었는지 확인
        else if (qm.completedQuests.Contains(questToGive))
        {
            // 5. (C) 완료된 퀘스트 -> 완료 후 대화
            ShowDialogue("도와주셔서 정말 감사합니다.");
        }
        // 6. 퀘스트가 '새 퀘스트'일 때
        else
        {
            // 7. (D) 새 퀘스트 -> 퀘스트 수락
            qm.AcceptQuest(questToGive);
            ShowDialogue(questToGive.description);
        }
    }

    private void ShowDialogue(string message)
    {
        // 1. 대화창 패널 활성화
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // 2. 텍스트 설정
        if (npcNameText != null)
        {
            npcNameText.text = npcName; // 설정한 NPC 이름
        }
        if (dialogueText != null)
        {
            dialogueText.text = message; // 퀘스트 상황별 메시지
        }
    }
    private void CloseDialogue()
    {
        // 1. 대화창 패널 비활성화
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        // 2. 카메라 원래대로
        EndDialogueCamera();
    }
    private void StartDialogueCamera()
    {
        if (dialogueCamera != null)
        {
            // 기본 VCam(10)보다 높은 20으로 설정
            dialogueCamera.Priority = 20;
        }
    }

    public void EndDialogueCamera()
    {
        if (dialogueCamera != null)
        {
            dialogueCamera.Priority = defaultPriority;
        }
    }
}