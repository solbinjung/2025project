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

    [SerializeField] private string locationName = "½£";

    private PlayerController playerController;
    private bool popupActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();

            UIManager.Instance.ShowSceneChangePopup(locationName, LoadTargetScene, CancelMove);
            popupActive = true;

            Time.timeScale = 0f;

            if (playerController != null)
            {
                playerController.StopMovement();
                playerController.CanControl = false;
            }
        }
    }

    private void LoadTargetScene()
    {
        popupActive = false; 
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void CancelMove()
    {
        popupActive = false; 
        Time.timeScale = 1f;

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
                UIManager.Instance.ClosePopup();
                CancelMove();
            }
            playerController = null;
        }
    }
}