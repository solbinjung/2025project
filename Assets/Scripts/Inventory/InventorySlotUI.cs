using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    public void ClearSlot()
    {
        itemIcon.enabled = false;
        amountText.enabled = false;
    }

    public void DrawSlot(ItemSlot slotData)
    {
        if (slotData == null || slotData.item == null)
        {
            ClearSlot();
            return;
        }
        // 아이콘 표시
        itemIcon.enabled = true;
        itemIcon.sprite = slotData.item.icon;

        // 수량 텍스트 표시
        if (slotData.item.isStackable && slotData.amount > 1)
        {
            amountText.enabled = true;
            amountText.text = slotData.amount.ToString();
        }
        else // 겹칠 수 없거나 1개일 때는 수량 텍스트 숨김
        {
            amountText.enabled = false;
        }
    }
}