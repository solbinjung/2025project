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
    public void ToggleGameStopPanel()
    {
        isGameStopped = !isGameStopped;

        if (isGameStopped) // 게임을 멈춰야 한다면
        {
            if (gameStopPanel != null) gameStopPanel.SetActive(true);
            Time.timeScale = 0f;

            //// (선택) 핫바 숨기기
            //if (InventoryUIManager.Instance != null)
            //    InventoryUIManager.Instance.SetPlayerHUDActive(false);
        }
        else // 게임을 재개해야 한다면
        {
            if (gameStopPanel != null) gameStopPanel.SetActive(false);
            Time.timeScale = 1f;

            //// (선택) 핫바 다시 보이기
            //if (InventoryUIManager.Instance != null)
            //    InventoryUIManager.Instance.SetPlayerHUDActive(true);
        }
    }
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
    }
    public void LoadSceneWithLoadingScreen(string sceneName)
    {
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

    public void OnClick_GoToMainMenu()
    {
        Debug.Log("Going to MainMenuScene...");

        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.RevivePlayer();
        }

        Time.timeScale = 1f;

        LoadSceneWithLoadingScreen("MainMenuScene");
    }

    public void OnClick_ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}