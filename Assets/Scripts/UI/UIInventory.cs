using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemsGrid;
    [SerializeField] private Image dragIconImage;

    private InventorySlotUI[] slots;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        slots = itemsGrid.GetComponentsInChildren<InventorySlotUI>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Initialize(i);
        }

        InventoryManager.OnInventoryChanged += UpdateInventoryUI;

        if (dragIconImage != null)
        {
            ItemDragHandler.dragIcon = dragIconImage;
            ItemDragHandler.dragIcon.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UpdateInventoryUI();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        InventoryManager.OnInventoryChanged -= UpdateInventoryUI;
    }

    public void UpdateInventoryUI()
    {
        // InventoryManager의 데이터 리스트
        List<ItemSlot> itemList = InventoryManager.Instance.inventory;


        // 모든 UI 슬롯 순회
        for (int i = 0; i < slots.Length; i++)
        {
            // 아이템이 있으면 DrawSlot
            if (itemList[i].item != null)
            {
                slots[i].DrawSlot(itemList[i]);
            }
            else
            {
                // 아이템이 없으면 ClearSlot
                slots[i].ClearSlot();
            }
        }
    }
    public void OpenInventoryPanel()
    {
        gameObject.SetActive(true);
        UpdateInventoryUI();
    }
    public void CloseInventoryPanel()
    {
        gameObject.SetActive(false);
    }

    public void ToggleInventoryPanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            UpdateInventoryUI();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}