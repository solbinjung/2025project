using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int amount;

    public ItemSlot(ItemData _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
}


public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20; // 인벤토리 슬롯 최대 개수

    public List<ItemSlot> inventory = new List<ItemSlot>();


    // 아이템 추가
    public void AddItem(ItemData itemToAdd, int amountToAdd)
    {
        // 1. 겹칠 수 있는 아이템인지 확인
        if (itemToAdd.isStackable)
        {
            // 2. 인벤토리에 이미 같은 아이템이 있는지 검색
            ItemSlot existingSlot = inventory.Find(slot => slot.item == itemToAdd);

            if (existingSlot != null)
            {
                // 3-1. 있다면 해당 슬롯의 수량만 증가
                existingSlot.amount += amountToAdd;
                Debug.Log($"{itemToAdd.itemName} {amountToAdd}개 추가. 총: {existingSlot.amount}개");
            }
            else
            {
                // 3-2. 없다면 새 슬롯을 만들어서 추가 (인벤토리가 꽉 차지 않았을 때)
                if (inventory.Count < inventorySize)
                {
                    inventory.Add(new ItemSlot(itemToAdd, amountToAdd));
                    Debug.Log($"{itemToAdd.itemName} {amountToAdd}개 새로 추가.");
                }
                else
                {
                    Debug.LogWarning("인벤토리가 꽉 찼습니다!");
                }
            }
        }
        else // 겹칠 수 없는 아이템 (장비 등)
        {
            // 인벤토리가 꽉 차지 않았다면 개수만큼 슬롯을 새로 추가
            if (inventory.Count < inventorySize)
            {
                for (int i = 0; i < amountToAdd; i++)
                {
                    inventory.Add(new ItemSlot(itemToAdd, 1));
                }
                Debug.Log($"{itemToAdd.itemName} {amountToAdd}개 새로 추가.");
            }
            else
            {
                Debug.LogWarning("인벤토리가 꽉 찼습니다!");
            }
        }
        // TODO: 여기에 인벤토리 UI를 업데이트하는 코드를 호출하세요.
        // 예: UIManager.Instance.UpdateInventoryUI();
    }

    // 아이템 제거
    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        ItemSlot slotToRemove = inventory.Find(slot => slot.item == itemToRemove);

        if (slotToRemove != null)
        {
            slotToRemove.amount -= amountToRemove;

            // 제거 후 수량이 0 이하가 되면 인벤토리에서 슬롯 자체를 삭제
            if (slotToRemove.amount <= 0)
            {
                inventory.Remove(slotToRemove);
                Debug.Log($"{itemToRemove.itemName} 아이템이 인벤토리에서 제거되었습니다.");
            }
            else
            {
                Debug.Log($"{itemToRemove.itemName} {amountToRemove}개 사용. 남은 수량: {slotToRemove.amount}개");
            }
        }
        else
        {
            Debug.LogWarning($"{itemToRemove.itemName} 아이템을 소지하고 있지 않습니다.");
        }
        // TODO: 여기에 인벤토리 UI를 업데이트하는 코드를 호출하세요.
    }


    // 아이템 소지 확인
    public bool HasItem(ItemData itemToFind, int requiredAmount = 1)
    {
        ItemSlot slot = inventory.Find(s => s.item == itemToFind);

        if (slot != null && slot.amount >= requiredAmount)
        {
            return true;
        }

        return false;
    }
}