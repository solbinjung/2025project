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


    [SerializeField] private float minimumLoadingTime = 1.5f;
    void Start()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameEndingPanel != null) gameEndingPanel.SetActive(false);
    }
    public void ShowGameOverPanel()
    {
        Debug.Log("UI: 게임 오버 패널 표시");

        GameObject sceneCanvas = GameObject.FindWithTag("SceneCanvas");
        if (sceneCanvas != null)
        {
            sceneCanvas.SetActive(false);
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void ShowGameEndingPanel()
    {
        Debug.Log("UI: 게임 엔딩 패널 표시");

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
            Debug.LogError("Loading Panel이 UIManager에 할당되지 않았습니다! 씬을 그냥 로드합니다.");
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
    // '불러오기' 버튼
    public void OnClick_LoadGame()
    {
        Debug.Log("MainScene(마을)으로 이동합니다.");

        LoadSceneWithLoadingScreen("MainScene");
    }
    // '메인으로' 버튼
    public void OnClick_GoToMainMenu()
    {
        Debug.Log("MainMenuScene으로 이동합니다.");
        LoadSceneWithLoadingScreen("MainMenuScene");
    }
    // '종료하기' 버튼
    public void OnClick_ExitGame() 
    {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}