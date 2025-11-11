using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    #region Singleton
    public static InventoryUIManager Instance { get; private set; }
    #endregion

    [Header("UI 스크립트 참조")]
    [SerializeField] private UIInventory uiInventory;
    [SerializeField] private UIHotbar uiHotbar;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; 
        }
        InventoryManager.OnInventoryChanged += RedrawAll;
    }

    private void Start()
    {
        if (uiInventory != null)
        {
            uiInventory.CloseInventoryPanel();
        }
    }

    private void OnDestroy()
    {
        InventoryManager.OnInventoryChanged -= RedrawAll;
    }
    public void RedrawAll()
    {
        Debug.Log("InventoryUIManager: RedrawAll() 호출됨.");

        // 인벤토리 패널 새로고침
        if (uiInventory != null)
        {
            uiInventory.UpdateInventoryUI();
        }

        // 핫바 패널 새로고침
        if (uiHotbar != null)
        {
            uiHotbar.UpdateHotbarUI();
        }
    }
    // UIManager가 게임오버/엔딩 시 호출
    public void SetPlayerHUDActive(bool isActive)
    {
        //if (uiHotbar != null)
        //{
        //    uiHotbar.gameObject.SetActive(isActive);
        //}

        // 만약 게임 오버/엔딩 시 인벤토리가 열려있었다면 강제로 닫기
        if (!isActive && uiInventory != null)
        {
            uiInventory.CloseInventoryPanel();
        }
    }
}