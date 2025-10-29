using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string locationName = "숲";

    [Header("Popup Prefab")]
    [SerializeField] private GameObject sceneChangePopupPrefab;

    private PlayerController playerController;

    private GameObject currentPopupInstance = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentPopupInstance == null)
        {
            playerController = other.GetComponent<PlayerController>();

            Canvas targetCanvas = GameObject.FindWithTag("SceneCanvas")?.GetComponent<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("[ChangeScene] UI를 표시할 Canvas를 찾을 수 없습니다!");
                return;
            }

            currentPopupInstance = Instantiate(sceneChangePopupPrefab, targetCanvas.transform);
            SceneChangePopup popupScript = currentPopupInstance.GetComponent<SceneChangePopup>();

            if (popupScript != null)
            {
                popupScript.SetupPopup(locationName, sceneToLoad, playerController);
            }
            else
            {
                Debug.LogError("[ChangeScene] SceneChange Panel 프리팹에 SceneChangePopup 스크립트가 없습니다!");
                Destroy(currentPopupInstance);
                return;
            }

            Time.timeScale = 0f;

            if (playerController != null)
            {
                playerController.StopMovement();
                playerController.CanControl = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentPopupInstance != null)
            {
                SceneChangePopup popupScript = currentPopupInstance.GetComponent<SceneChangePopup>();
                if (popupScript != null)
                {
                    popupScript.OnNoClicked();
                }
                else
                {
                    Destroy(currentPopupInstance);
                }
            }
            playerController = null;
            currentPopupInstance = null; 
        }
    }
}