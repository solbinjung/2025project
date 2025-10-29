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

    [Header("UI References")]
    [SerializeField] private GameObject interactPrompt;
    private GameObject dialoguePanel;
    private TextMeshProUGUI npcNameText;
    private TextMeshProUGUI dialogueText;
    private Button nextButton; 

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera dialogueCamera;

    private bool playerIsNearby = false;
    private int defaultPriority;

    private PlayerController playerController;
    private bool isInteracting = false; 

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
        if (dialogueCamera != null)
        {
            defaultPriority = dialogueCamera.Priority;
        }

        if (UIManager.Instance != null)
        {
            dialoguePanel = UIManager.Instance.dialoguePanel;
            npcNameText = UIManager.Instance.npcNameText;
            dialogueText = UIManager.Instance.dialogueText;
            nextButton = UIManager.Instance.nextButton;

            // 찾아온 UI 요소들이 null이 아닌지 확인 (UIManager 설정 누락 방지)
            if (dialoguePanel == null || npcNameText == null || dialogueText == null || nextButton == null)
            {
                Debug.LogError($"[QuestNPC] UIManager에 Dialogue UI 요소 중 일부가 연결되지 않았습니다!");
            }
            else
            {
                // UI 요소들을 찾았으면 초기 상태 설정
                dialoguePanel.SetActive(false);
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(CloseDialogue);
            }
        }
        else
        {
            Debug.LogError("[QuestNPC] UIManager.Instance를 찾을 수 없습니다!");
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
            playerController = other.GetComponent<PlayerController>();
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
            playerController = null;
        }
    }

    private void Update()
    {
        if (playerIsNearby && Input.GetKeyDown(KeyCode.F) && !isInteracting && playerController != null)
        {
            Interact();
        }
    }

    private void Interact()
    {
        isInteracting = true;
        playerController.StopMovement(); 
        playerController.CanControl = false; 

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        StartDialogueCamera();

        QuestManager qm = QuestManager.Instance;
        ActiveQuest activeQuest = qm.activeQuests.Find(q => q.data == questToGive);

        if (activeQuest != null) // 퀘스트가 '진행 중'일 때
        {
            // 1. 목표 달성 & '보고 가능' 상태인가? (isReadyToComplete 체크)
            if (activeQuest.isReadyToComplete)
            {
                qm.CompleteQuest(questToGive); // 퀘스트 완료 처리 (보상 지급)
                ShowDialogue("훌륭하군요! 여기 보상입니다."); // 완료 대화
            }
            // 2. 아직 진행 중인가?
            else
            {
                ShowDialogue("아직 퀘스트를 완료하지 못했군요. 어서 가보세요."); // 진행 중 대화
            }
        }
        else if (qm.completedQuests.Contains(questToGive)) // 퀘스트가 '이미 완료'되었는지 확인
        {
            // 3. 이미 완료한 퀘스트인가?
            ShowDialogue("도와주셔서 정말 감사합니다."); // 완료 후 대화
        }
        else // 퀘스트가 '새 퀘스트'일 때
        {
            // 4. 새로운 퀘스트인가?
            qm.AcceptQuest(questToGive);
            ShowDialogue(questToGive.description); // 퀘스트 수락 (설명문 표시)
        }
    }

    private void ShowDialogue(string message)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
        if (npcNameText != null)
        {
            npcNameText.text = npcName;
        }
        if (dialogueText != null)
        {
            dialogueText.text = message;
        }
    }

    private void CloseDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        EndDialogueCamera();

        if (playerController != null)
        {
            playerController.CanControl = true;
        }
        isInteracting = false;

        if (playerIsNearby && interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private void StartDialogueCamera()
    {
        if (dialogueCamera != null)
        {
            dialogueCamera.Priority = 20;
        }
    }

    private void EndDialogueCamera()
    {
        if (dialogueCamera != null)
        {
            dialogueCamera.Priority = defaultPriority;
        }
    }
}