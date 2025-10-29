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
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton; 


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
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
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

        if (activeQuest != null)
        {
            if (activeQuest.IsAllObjectivesCompleted())
            {
                qm.CompleteQuest(questToGive);
                ShowDialogue("훌륭하군요! 여기 보상입니다.");
            }
            else
            {
                ShowDialogue("아직 퀘스트를 완료하지 못했군요. 어서 가보세요.");
            }
        }
        else if (qm.completedQuests.Contains(questToGive))
        {
            ShowDialogue("도와주셔서 정말 감사합니다.");
        }
        else
        {
            qm.AcceptQuest(questToGive);
            ShowDialogue(questToGive.description);
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