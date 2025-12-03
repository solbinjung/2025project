using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneChangePopup : MonoBehaviour
{
    [Header("UI References (Internal)")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private string sceneToLoad;
    private PlayerController playerController;

    public void SetupPopup(string locationName, string targetScene, PlayerController pc)
    {
        locationName = locationName ?? "새로운 지역"; 
        questionText.text = $"'{locationName}'(으)로 이동하시겠습니까?";

        sceneToLoad = targetScene;
        playerController = pc;

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(OnYesClicked);

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void OnYesClicked()
    {
        Debug.Log(sceneToLoad + " 씬으로 이동을 시작합니다.");

        if (yesButton != null) yesButton.interactable = false;
        if (noButton != null) noButton.interactable = false;

        gameObject.SetActive(false);

        if (playerController != null)
        {
            playerController.CanControl = true;
        }

        UIManager.Instance.LoadSceneWithLoadingScreen(sceneToLoad);
    }

    public void OnNoClicked()
    {
        if (playerController != null)
        {
            playerController.CanControl = true;
        }
        Destroy(gameObject);
    }
}