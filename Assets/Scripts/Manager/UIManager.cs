using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR 
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameEndingPanel;
    [SerializeField] private GameObject gameStopPanel;
    [SerializeField] private GameObject controlsPanel;

    private bool isGameStopped = false;

    [SerializeField] private float minimumLoadingTime = 1.5f;
    void Start()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameEndingPanel != null) gameEndingPanel.SetActive(false);
        if (gameStopPanel != null) gameStopPanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ((loadingPanel != null && loadingPanel.activeSelf) ||
                (gameOverPanel != null && gameOverPanel.activeSelf) ||
                (gameEndingPanel != null && gameEndingPanel.activeSelf))
            {
                return; 
            }
            ToggleGameStopPanel();
        }
    }
    // 일시정지 패널
    public void ToggleGameStopPanel()
    {
        isGameStopped = !isGameStopped;

        if (isGameStopped) // 게임 일시정지
        {
            if (gameStopPanel != null) gameStopPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else // 게임 일시정지 해제
        {
            if (gameStopPanel != null) gameStopPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    // 게임오버 패널
    public void ShowGameOverPanel()
    {
        Debug.Log("UI: Show GameOver Panel");

        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.SetPlayerHUDActive(false);
        }

        GameObject sceneCanvas = GameObject.FindWithTag("SceneCanvas");
        if (sceneCanvas != null)
        {
            sceneCanvas.SetActive(false);
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    // 게임엔딩 패널
    public void ShowGameEndingPanel()
    {
        Debug.Log("UI: Show GameEnding Panel");

        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.SetPlayerHUDActive(false);
            
        }

        GameObject sceneCanvas = GameObject.FindWithTag("SceneCanvas");
        if (sceneCanvas != null)
        {
            sceneCanvas.SetActive(false);
        }
        if (gameEndingPanel != null) gameEndingPanel.SetActive(true);

        StartCoroutine(EndGameSequence());
    }
    // 엔딩 화면 5초 후 메인으로
    private IEnumerator EndGameSequence()
    {
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(5f);

        OnClick_GoToMainMenu();
    }
    // 씬전환 로딩 패널
    public void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (sceneName != "MainMenuScene")
        {
            Debug.Log("저장 중...");
            SaveLoadManager.Instance.SaveGame();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameEndingPanel != null) gameEndingPanel.SetActive(false);

        StartCoroutine(LoadSceneAsync(sceneName));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingPanel == null)
        {
            Debug.LogError("Loading Panel is not assigned! Loading scene directly.");
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        float startTime = Time.realtimeSinceStartup;

        loadingPanel.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        float remainingTime = minimumLoadingTime - elapsedTime;

        if (remainingTime > 0)
        {
            yield return new WaitForSecondsRealtime(remainingTime);
        }
        loadingPanel.SetActive(false);
    }
    
    // ===============버튼===============
    // 메인메뉴로 이동 버튼
    public void OnClick_GoToMainMenu()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.RevivePlayer();
        }

        Time.timeScale = 1f;

        LoadSceneWithLoadingScreen("MainMenuScene");
    }
    // 조작 방법 창 열기 버튼
    public void OnClick_ControlsToggle()
    {
        Debug.Log("조작 방법 토글 버튼 클릭");

        bool isCurrentlyActive = controlsPanel.activeSelf;

        controlsPanel.SetActive(!isCurrentlyActive);
    }
    // 조작 방법 창 닫기 버튼
    public void OnClick_ControlsEnd()
    {
        Debug.Log("조작 방법 끄기 버튼 클릭");

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }
    // 게임 종료 버튼
    public void OnClick_ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}