using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemsGrid;

    private InventorySlotUI[] slots;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Start()
    {
        slots = itemsGrid.GetComponentsInChildren<InventorySlotUI>();
        InventoryManager.OnInventoryChanged += UpdateInventoryUI;
        UpdateInventoryUI();

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        InventoryManager.OnInventoryChanged -= UpdateInventoryUI;
    }

    private void UpdateInventoryUI()
    {
        List<ItemSlot> itemList = InventoryManager.Instance.inventory;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemList.Count)
            {
                slots[i].DrawSlot(itemList[i]);
            }
            else
            {
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
