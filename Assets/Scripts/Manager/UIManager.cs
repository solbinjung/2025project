using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Scene Change Popup References")]
    public GameObject popupPanel;
    public TextMeshProUGUI questionText;
    public Button yesButton;
    public Button noButton;

    [Header("Dialogue UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    // (나중에 다른 전역 UI 요소들 참조 추가 가능: 미니맵, 설정창 등)

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public void ShowSceneChangePopup(string locationName, UnityEngine.Events.UnityAction onYesAction, UnityEngine.Events.UnityAction onNoAction)
    {
        if (popupPanel == null || questionText == null || yesButton == null || noButton == null)
        {
            Debug.LogError("UIManager에 씬 전환 팝업 UI 요소들이 연결되지 않았습니다!");
            return;
        }

        questionText.text = $"'{locationName}'(으)로 이동하시겠습니까?";

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(onYesAction); 
        yesButton.onClick.AddListener(ClosePopup);  

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(onNoAction);   
        noButton.onClick.AddListener(ClosePopup); 

        // 3. 팝업창 활성화
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
}