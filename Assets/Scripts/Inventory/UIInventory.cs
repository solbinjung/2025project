using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private GameObject itemsGrid;

    private InventorySlotUI[] slots;

    void Start()
    {
        slots = itemsGrid.GetComponentsInChildren<InventorySlotUI>();

        InventoryManager.OnInventoryChanged += UpdateInventoryUI;

        UpdateInventoryUI();
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
}
