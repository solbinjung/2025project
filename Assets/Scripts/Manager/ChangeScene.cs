using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;              
using TMPro;                       

public class ChangeScene : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string locationName = "숲";

    private GameObject popupPanel;
    private TextMeshProUGUI questionText;
    private Button yesButton;
    private Button noButton;

    private PlayerController playerController;
    private bool popupActive = false;

    private void Start()
    {
        // 1. 이름으로 'Canvas_Main'을 찾음 (이미지 기준)
        GameObject sceneCanvasObj = GameObject.Find("Canvas_Main");
        if (sceneCanvasObj != null)
        {
            // 2. Canvas 아래에서 이름으로 UI 요소들을 찾음
            // (주의: Find는 직속 자식만 찾으므로, 경로가 다르면 null 반환)
            popupPanel = sceneCanvasObj.transform.Find("SceneChange Panel")?.gameObject;

            if (popupPanel != null)
            {
                // 3. 찾은 패널 '내부'에서 컴포넌트 찾기 (더 안전)
                questionText = popupPanel.GetComponentInChildren<TextMeshProUGUI>();
                // (이름으로 찾으려면 popupPanel.transform.Find("QuestionText")...)

                // 버튼은 이름으로 찾는 것이 더 나을 수 있음
                Button[] buttons = popupPanel.GetComponentsInChildren<Button>();
                foreach (Button btn in buttons)
                {
                    if (btn.name == "YesButton") yesButton = btn;
                    else if (btn.name == "NoButton") noButton = btn;
                }

                // 4. 버튼 리스너 설정 및 초기화
                if (yesButton != null)
                {
                    yesButton.onClick.RemoveAllListeners();
                    yesButton.onClick.AddListener(LoadTargetScene);
                }
                else Debug.LogError("[ChangeScene] YesButton을 찾을 수 없습니다!");

                if (noButton != null)
                {
                    noButton.onClick.RemoveAllListeners();
                    noButton.onClick.AddListener(CancelMove);
                }
                else Debug.LogError("[ChangeScene] NoButton을 찾을 수 없습니다!");

                popupPanel.SetActive(false);
            }
            else
            {
                Debug.LogError($"[ChangeScene] 'Canvas_Main' 아래에서 'SceneChange Panel'을 찾을 수 없습니다! (이름/위치 확인)");
            }
        }
        else
        {
            Debug.LogError($"[ChangeScene] 씬에서 'Canvas_Main' 오브젝트를 찾을 수 없습니다!");
        }
        // --- 찾기 끝 ---
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !popupActive)
        {
            playerController = other.GetComponent<PlayerController>();

            if (popupPanel != null && questionText != null)
            {
                questionText.text = $"'{locationName}'(으)로 이동하시겠습니까?";
                popupPanel.SetActive(true);
                popupActive = true;

                // Time.timeScale = 0f; // (선택) 시간 멈춤

                if (playerController != null)
                {
                    playerController.StopMovement();
                    playerController.CanControl = false;
                }
            }
        }
    }

    private void LoadTargetScene() 
    {
        popupActive = false;
        Time.timeScale = 1f;

        if (popupPanel != null) popupPanel.SetActive(false);

        SceneManager.LoadScene(sceneToLoad);
    }

    private void CancelMove()
    {
        popupActive = false;
        Time.timeScale = 1f;

        if (popupPanel != null) popupPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.CanControl = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupActive)
            {
                if (popupPanel != null) popupPanel.SetActive(false);
                CancelMove();
            }
            playerController = null;
        }
    }
}