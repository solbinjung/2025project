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

        GameObject sceneCanvasObj = GameObject.Find("Canvas_Main"); // 이름으로 캔버스 찾기
        if (sceneCanvasObj != null)
        {
            // Canvas 아래에서 이름으로 Panel 찾기
            dialoguePanel = sceneCanvasObj.transform.Find("Dialogue Panel")?.gameObject;
            if (dialoguePanel != null)
            {
                // Panel 내부에서 컴포넌트 찾기 (이름 대신 타입으로 - 더 안전)
                TextMeshProUGUI[] texts = dialoguePanel.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var textComp in texts)
                {
                    if (textComp.name == "NPCNameText") npcNameText = textComp;
                    else if (textComp.name == "DialogueText") dialogueText = textComp;
                }

                // Panel 내부에서 Button 찾기
                nextButton = dialoguePanel.GetComponentInChildren<Button>(); // (버튼이 하나라고 가정)
                // (만약 버튼 이름으로 찾아야 한다면 dialoguePanel.transform.Find("NextButton")...)

                // Null 체크 및 초기화
                if (npcNameText == null) Debug.LogError("[QuestNPC] NPCNameText 찾기 실패!");
                if (dialogueText == null) Debug.LogError("[QuestNPC] DialogeText 찾기 실패!");
                if (nextButton == null) Debug.LogError("[QuestNPC] NextButton 찾기 실패!");

                if (npcNameText != null && dialogueText != null && nextButton != null)
                {
                    dialoguePanel.SetActive(false);
                    nextButton.onClick.RemoveAllListeners();
                    nextButton.onClick.AddListener(CloseDialogue);
                }
            }
            else
            {
                Debug.LogError($"[QuestNPC] 'Canvas_Main' 아래에서 'Dialoge Panel'을 찾을 수 없습니다! (이름/위치 확인)");
            }
        }
        else
        {
            Debug.LogError($"[QuestNPC] 씬에서 'Canvas_Main' 오브젝트를 찾을 수 없습니다!");
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