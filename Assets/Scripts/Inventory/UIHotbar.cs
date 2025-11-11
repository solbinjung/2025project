using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 핫바 슬롯의 UI 요소들을 묶는 클래스
[System.Serializable]
public class HotbarSlotUI
{
    public Image itemIcon;
    public TextMeshProUGUI amountText;

    public void Clear()
    {
        itemIcon.enabled = false;
        amountText.enabled = false;
    }

    public void Draw(ItemSlot slotData)
    {
        itemIcon.enabled = true;
        itemIcon.sprite = slotData.item.icon;

        if (slotData.item.isStackable && slotData.amount > 1)
        {
            amountText.enabled = true;
            amountText.text = slotData.amount.ToString();
        }
        else
        {
            amountText.enabled = false;
        }
    }
}

public class UIHotbar : MonoBehaviour
{
    // Inspector에서 ASDZX 슬롯의 아이콘과 텍스트를 순서대로 (0~4) 연결
    public HotbarSlotUI[] hotbarSlotsUI = new HotbarSlotUI[5];

    void Start()
    {
        // 인벤토리가 변경될 때마다 핫바 UI도 업데이트
        InventoryManager.OnInventoryChanged += UpdateHotbarUI;

        // 게임 시작 시 초기 상태 업데이트
        UpdateHotbarUI();
    }

    private void OnDestroy()
    {
        InventoryManager.OnInventoryChanged -= UpdateHotbarUI;
    }

    public void UpdateHotbarUI()
    {
        List<ItemSlot> inventory = InventoryManager.Instance.inventory;

        for (int i = 0; i < hotbarSlotsUI.Length; i++) // 핫바 순회(5칸)
        {
            // 인벤토리에 i번째 아이템이 있고, 그 아이템이 null이 아니면
            if (i < inventory.Count && inventory[i] != null && inventory[i].item != null)
            {
                // 핫바 UI에 아이템 그리기
                hotbarSlotsUI[i].Draw(inventory[i]);
            }
            else
            {
                // 인벤토리 슬롯이 비어있으면 핫바 UI도 비우기
                hotbarSlotsUI[i].Clear();
            }
        }
    }
}