using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    private ItemSlot mySlotData;
    public int slotIndex { get; private set; }

    public void Initialize(int index)
    {
        slotIndex = index;
    }

    public void ClearSlot()
    {
        mySlotData = null;
        itemIcon.enabled = false;
        amountText.enabled = false;
    }

    public void DrawSlot(ItemSlot slotData)
    {
        mySlotData = slotData;

        if (slotData == null || slotData.item == null)
        {
            ClearSlot();
            return;
        }

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
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. 빈 슬롯이 아니면 드래그 시작
        if (mySlotData != null && mySlotData.item != null)
        {
            // 2. 드래그 핸들러에 '나의 인덱스'를 등록
            ItemDragHandler.draggedSlotIndex = slotIndex;
            ItemDragHandler.dragIcon.sprite = mySlotData.item.icon;
            ItemDragHandler.dragIcon.gameObject.SetActive(true);

            // 3. 원래 슬롯 아이콘 반투명
            itemIcon.color = new Color(1, 1, 1, 0.5f);
            amountText.color = new Color(1, 1, 1, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemDragHandler.dragIcon.gameObject.activeInHierarchy)
        {
            ItemDragHandler.dragIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. 드래그가 끝나면 드래그 정보를 초기화
        ItemDragHandler.draggedSlotIndex = -1;
        ItemDragHandler.dragIcon.gameObject.SetActive(false);

        // 2. 원래 슬롯 아이콘 불투명
        itemIcon.color = Color.white;
        amountText.color = Color.white;
    }

    /// (추가) 다른 아이템을 이 슬롯에 드롭했을 때
    public void OnDrop(PointerEventData eventData)
    {
        // 1. 드래그 중인 아이템의 '시작 인덱스'를 가져옴
        int sourceIndex = ItemDragHandler.draggedSlotIndex;

        // 2. 드래그 중인 아이템이 있고, 자기 자신이 아니라면
        if (sourceIndex != -1 && sourceIndex != slotIndex)
        {
            // 3. 인벤토리 매니저에게 슬롯을 교환(Swap)하라고 요청
            InventoryManager.Instance.SwapSlots(sourceIndex, slotIndex);
        }
    }
}