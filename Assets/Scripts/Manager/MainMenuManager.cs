using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리
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

    // '게임 시작' 버튼
    public void OnClick_GameStart()
    {
        Debug.Log("게임 시작 버튼 클릭");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        if (explanationPanel != null)
        {
            explanationPanel.SetActive(true);
            StartCoroutine(ShowExplanationText());
        }
    }
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
        SceneManager.LoadScene("StartScene");
    }

    // '불러오기' 버튼
    public void OnClick_LoadGame()
    {
        Debug.Log("불러오기 버튼 클릭");
    }

    // '조작 방법' 버튼
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

        // 유니티 에디터에서 실행 중일 경우
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        // 실제 빌드된 게임에서 실행 중일 경우
#else
        Application.Quit();
#endif
    }

    public void OnClick_ExplanationContinue()
    {
        Debug.Log("설명 확인 -> StartScene 로드 시작");
        SceneManager.LoadScene("StartScene");
    }

    public void OnClick_BackToMain()
    {
        Debug.Log("메인 메뉴로 복귀");
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (explanationPanel != null) explanationPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}