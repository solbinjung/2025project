using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor; 
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Explanation Text Settings")]
    [SerializeField] private GameObject[] explanationParagraphs;
    [SerializeField] private float paragraphDelay = 1.5f;
    [SerializeField] private float fadeInDuration = 1.0f;

    public Button loadButton;

    private void Start()
    {
        // 다른 패널들은 비활성화
        if (explanationPanel != null) explanationPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        // 게임 시작 시 저장 데이터가 있는지 확인
        if (SaveLoadManager.Instance.HasSaveData)
        {
            // 데이터가 있으면 버튼 활성화
            loadButton.interactable = true;
        }
        else
        {
            // 데이터가 없으면 버튼 비활성화
            loadButton.interactable = false;
        }
    }

    // 새 게임 버튼
    public void OnClick_GameStart()
    {
        Debug.Log("게임 시작 버튼 클릭");

        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.StartNewGame();
        }
        else
        {
            Debug.LogError("SaveLoadManager was not found!");
        }

        Time.timeScale = 1f;

        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.RevivePlayer();
        }
        else
        {
            Debug.LogWarning("플레이어를 부활시킬 수 없습니다.");
        }

        // 게임 오프닝 설명 텍스트 출력
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (explanationPanel != null)
        {
            explanationPanel.SetActive(true);
            StartCoroutine(ShowExplanationText());
        }
    }
    // 인트로 화면 설명 문단 시간차 출력
    private IEnumerator ShowExplanationText()
    {
        foreach (GameObject paragraph in explanationParagraphs)
        {
            CanvasGroup cg = paragraph.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                paragraph.SetActive(true); 
                cg.alpha = 0f; // 알파값 0
            }
            else
            {
                paragraph.SetActive(false);
                Debug.LogWarning(paragraph.name + "에 CanvasGroup 컴포넌트가 없습니다!");
            }
        }
        foreach (GameObject paragraph in explanationParagraphs)
        {
            CanvasGroup cg = paragraph.GetComponent<CanvasGroup>();
            if (cg == null) continue; 

            float timer = 0f;
            while (timer < fadeInDuration)
            {
                float progress = timer / fadeInDuration;
                cg.alpha = Mathf.Lerp(0, 1, progress);

                timer += Time.deltaTime;
                yield return null; 
            }
            cg.alpha = 1f; 

            yield return new WaitForSeconds(paragraphDelay);
        }
        Debug.Log("모든 설명 문단 표시 완료");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.LoadSceneWithLoadingScreen("MainScene");
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    // 불러오기 버튼
    public void OnClick_LoadGame()
    {
        // 데이터 파일 읽기 시도
        if (SaveLoadManager.Instance.LoadGame())
        {
            // 저장된 데이터에서 씬 이름 가져오기
            string savedSceneName = SaveLoadManager.Instance.currentSaveData.sceneName;

            Debug.Log($"저장된 씬({savedSceneName})으로 이동합니다.");

            // 로딩 화면을 띄우며 씬 이동
            if (UIManager.Instance != null)
            {
                UIManager.Instance.LoadSceneWithLoadingScreen(savedSceneName);
            }
            else
            {
                // UIManager가 없을 경우
                SceneManager.LoadScene(savedSceneName);
            }
        }
        else
        {
            Debug.LogError("파일 로드 실패");
            loadButton.interactable = false; // 에러 발생 시 버튼 끄기
        }
    }

    // 조작 방법 버튼
    public void OnClick_ControlsToggle()
    {
        Debug.Log("조작 방법 토글 버튼 클릭");

        bool isCurrentlyActive = controlsPanel.activeSelf;

        controlsPanel.SetActive(!isCurrentlyActive);
    }

    public void OnClick_ControlsEnd()
    {
        Debug.Log("조작 방법 끄기 버튼 클릭");

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }
    // '게임 종료' 버튼
    public void OnClick_ExitGame()
    {
        Debug.Log("게임 종료 버튼 클릭");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    //public void OnClick_BackToMain()
    //{
    //    Debug.Log("메인 메뉴로 복귀");
    //    if (controlsPanel != null) controlsPanel.SetActive(false);
    //    if (explanationPanel != null) explanationPanel.SetActive(false);
    //    if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    //}
}